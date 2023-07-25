from flask import Flask, request, json

import io
import numpy as np
from keras.preprocessing import image
from keras.applications.resnet50 import ResNet50, preprocess_input, decode_predictions
from keras.models import load_model



app = Flask(__name__)


def generate_response(data, code):
    return app.response_class(
        response=json.dumps(data),
        status=code,
        mimetype='application/json'
    )

# For debugging
@app.route('/ping', methods=['GET'])
def ping_pong():
    return generate_response({'message': 'Pong!'}, 200)


# key: model name, value: {'size': input size, 'model': model}
detectors = dict()
# Initialize deep learning models for image detection
def init_detection_model():
    # TODO: Initialize image detection models
    detectors['resnet50'] = {'size': 224, 'model': ResNet50(weights='imagenet')}

@app.route('/detect', methods=['POST'])
def detect():
    # Check if the post request has the 'image' part
    if 'image' not in request.files:
        return generate_response({'message': 'No file part in the request'}, 400)

    # Check if a valid model is given
    model = request.args.get('model')
    if not model:
        return generate_response({'message': 'Pick an image detection model'}, 400)
    elif model not in detectors.keys():
        return generate_response({'message': f'Model {model} is not supported'}, 400)

    # TODO: Object Detection. You need to choose models that output bounding boxes
    ######################## Example ##########################
    img = request.files['image'].read()
    img = image.load_img(io.BytesIO(img), target_size=(detectors[model]['size'], detectors[model]['size']))
    # Convert the image to a numpy array
    x = image.img_to_array(img)
    # Add a fourth dimension (since Keras expects a list of images)
    x = np.expand_dims(x, axis=0)
    # Preprocess the input image array
    x = preprocess_input(x)
    # Make a prediction
    preds = detectors[model]['model'].predict(x)
    results = decode_predictions(preds)
    # Prepare the results for JSON response
    data = {'predictions': []}
    for (imagenetID, label, prob) in results[0]:
        r = {'label': label, 'probability': float(prob)}
        data['predictions'].append(r)
    # NOTE: Take into consideration that image is resized. Ask Mansi to work around that.
    ######################## Example ##########################

    return generate_response(data, 200)


# key: model name, value: {'lang': [from]-[to], 'model': model}
translators = dict()
# Initialize deep learming models for translation
def init_translation_model():
    # TODO: Initialize translation models
    # e.g. translators['s2s'] = { 'en-fr': load_model('s2s.h5') }
    model = load_model('s2s.h5')
    translators['s2s'] = { 'en-fr': model }
    pass

@app.route('/translate', methods=['POST'])
def translate():
    # Check if 'from' is given
    orig = request.args.get('from')
    if not orig:
        return generate_response({'message': 'Original language has to be provided via parameter'}, 400)

    # Check if 'to' is given
    target = request.args.get('to')
    if not target:
        return generate_response({'message': 'Target language has to be provided via parameter'}, 400)
    
    # if key in translators[model].keys():
    #     # For the sake of this example, we'll just assume that the loaded model's predict
    #     # function directly returns a translated string. In reality, you'd probably need to
    #     # preprocess the text, run it through the model, then postprocess the result.
    #     translation = translators[model][key].predict(text)
    #     data = { 'translation': translation }
    #     return generate_response(data, 200)
    # else:
    #     return generate_response({'message': f'{orig} to {target} translation is not supported'}, 400)


    # Check if a valid 'model' is given
    model = request.args.get('model')
    if not model:
        return generate_response({'message': 'Pick a translation model'}, 400)
    elif model not in translators.keys():
        return generate_response({'message': f'Model {model} is not supported'}, 400)

    key = orig + '-' + target
    if key not in translators[model]:
        return generate_response({'message': f'{orig} to {target} translation is not supported'}, 400)

    # Assuming the text to be translated is sent in JSON format
    text = request.json.get('text', '')

    # For the sake of this example, we'll just assume that the loaded model's predict
    # function directly returns a translated string. In reality, you'd probably need to
    # preprocess the text, run it through the model, then postprocess the result.
    translation = translators[model][key].predict(text)
    data = { 'translation': translation }
    
    return generate_response(data, 200)  


    # Translate the text
    text = request.json['text']
    key = orig + '-' + target
    if key in translators[model].keys():
        # TODO: Translation.
        # e.g. translation = translators[model][key].predict()
        data = { 'translation': translation }
        return generate_response(data, 200)
    else:
        return generate_response({'message': f'{orig} to {target} translation is not supported'}, 400)


# Run it with "python3 main.py"
if __name__ == "__main__":
    init_translation_model()
    init_detection_model()

    app.run(host='0.0.0.0', port=80)
