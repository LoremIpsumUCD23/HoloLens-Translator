from flask import Flask, request, json
import os
import io

# ML imports
import numpy as np
from keras.preprocessing import image
from keras.applications.resnet50 import ResNet50, preprocess_input, decode_predictions
from keras.models import load_model

# logging imports
import google.cloud.logging
import logging

if os.getenv('ENV', default='local') == 'gcp': client = google.cloud.logging.Client()


# key: model name, value: {'size': input size, 'model': model}
detectors = dict()
# Initialize deep learning models for image detection
def init_detection_model():
    # TODO: Add more models
    detectors['resnet50'] = {'size': 224, 'model': ResNet50(weights='imagenet')}

# key: model name, value: {'lang': [from]-[to], 'model': model}
translators = dict()
# Initialize deep learming models for translation
def init_translation_model():
    # TODO: Add more models
    model = load_model('s2s.h5')
    translators['s2s'] = { 'en-fr': model }


def create_app():
    app = Flask(__name__)

    init_detection_model()
    logging.info('Object Detection Models Loaded')

    init_translation_model()
    logging.info('Translation Models Loaded')


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


    @app.route('/detect', methods=['POST'])
    def detect():
        # Check if the post request has the 'image' part
        if 'image' not in request.files:
            return generate_response({'message': 'No image part in the request'}, 400)

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
            if float(prob) > 0.2:
                # these keys are fixed. change the values.
                r = {'label': label, 'confidence': float(prob), 'x': 1, 'y': 1, 'w': 1, 'h': 1}
                data['predictions'].append(r)
        # NOTE: Take into consideration that image is resized. Ask Mansi to work around that.
        ######################## Example ##########################

        return generate_response(data, 200)


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

        # Check if a valid 'model' is given
        model = request.args.get('model')
        if not model:
            return generate_response({'message': 'Pick a translation model'}, 400)
        elif model not in translators.keys():
            return generate_response({'message': f'Model {model} is not supported'}, 400)

        # Check if translation language pair is supported by the specified model
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

    return app


# Run it with "python3 main.py"
if __name__ == "__main__":
    app = create_app()
    app.run(host='0.0.0.0', port=8080)
