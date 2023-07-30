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
        # [Your translation code here]

    return app

if __name__ == "__main__":
    app = create_translation_service()
    app.run(host='0.0.0.0', port=8082)
