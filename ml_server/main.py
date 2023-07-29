from flask import Flask, request, json
import os
import io

# ML imports
import numpy as np
from keras.preprocessing import image
from keras.applications.resnet50 import ResNet50, preprocess_input, decode_predictions

# For MarianMT model
from transformers import MarianMTModel, MarianTokenizer

# logging imports
import google.cloud.logging
import logging

if os.getenv('ENV', default='local') == 'gcp':
    client = google.cloud.logging.Client()

# key: model name, value: {'size': input size, 'model': model}
detectors = dict()

# Initialize deep learning models for image detection
def init_detection_model():
    detectors['resnet50'] = {'size': 224, 'model': ResNet50(weights='imagenet')}

# key: model name, value: {'name': model identifier, 'model': model, 'tokenizer': tokenizer}
translators_marian = dict()

def init_marian_translation_model():
    model_name = 'Helsinki-NLP/opus-mt-en-fr'
    model = MarianMTModel.from_pretrained(model_name)
    tokenizer = MarianTokenizer.from_pretrained(model_name)

    translators_marian['opus-mt-en-fr'] = {'name': model_name, 'model': model, 'tokenizer': tokenizer}

def create_app():
    app = Flask(__name__)

    init_detection_model()
    logging.info('Object Detection Models Loaded')

    init_marian_translation_model()
    logging.info('Marian Translation Models Loaded')

    def generate_response(data, code):
        return app.response_class(
            response=json.dumps(data),
            status=code,
            mimetype='application/json'
        )

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

        orig = request.args.get('from')
        target = request.args.get('to')
        model = request.args.get('model')
        text = request.json.get('text', '')

        if model in translators_marian.keys():
            marian_model = translators_marian[model]['model']
            marian_tokenizer = translators_marian[model]['tokenizer']
            inputs = marian_tokenizer.encode(text, return_tensors="pt", max_length=512, truncation=True)
            outputs = marian_model.generate(inputs, max_length=512, num_return_sequences=1)
            translation = marian_tokenizer.decode(outputs[0], skip_special_tokens=True)
        else:
            return generate_response({'message': f'Model {model} is not supported'}, 400)

        data = {'translation': translation}
        return generate_response(data, 200)

    return app

if __name__ == "__main__":
    app = create_app()
    app.run(host='0.0.0.0', port=8080)
