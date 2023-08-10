from flask import Flask, request, json

# For MarianMT model
from transformers import MarianMTModel, MarianTokenizer

# key: model name, value: {'name': model identifier, 'model': model, 'tokenizer': tokenizer}
translators_marian = dict()

def init_marian_translation_model():
    model_name = 'Helsinki-NLP/opus-mt-en-fr'
    model = MarianMTModel.from_pretrained(model_name)
    tokenizer = MarianTokenizer.from_pretrained(model_name)

    translators_marian['opus-mt-en-fr'] = {'name': model_name, 'model': model, 'tokenizer': tokenizer}

def create_translation_service():
    app = Flask("TranslationService")

    init_marian_translation_model()
    print('Marian Translation Models Loaded')

    @app.route('/translate', methods=['POST'])
    def translate():
        try:
            # Get the text from the request
            text = request.json['text']
            lang = request.json.get('lang', 'opus-mt-en-fr') # You can specify the language model if you have multiple

            # Get the corresponding model and tokenizer
            translator = translators_marian[lang]
            model = translator['model']
            tokenizer = translator['tokenizer']

            # Tokenize the text and generate translation
            inputs = tokenizer.encode(text, return_tensors="pt", max_length=512, truncation=True)
            outputs = model.generate(inputs, max_length=512, num_return_sequences=1)
            decoded = tokenizer.decode(outputs[0], skip_special_tokens=True)

            # Return the translation
            return json.jsonify({'translation': decoded})
        except Exception as e:
            # Handle errors
            return json.jsonify({'error': str(e)}), 400

    return app

if __name__ == "__main__":
    app = create_translation_service()
    app.run(host='0.0.0.0', port=8082)
