from flask import Flask, request, json
import os
import io

from externals import MongoDBClient

# logging imports
import google.cloud.logging
import logging


if os.getenv('ENV', default='local') == 'gcp': client = google.cloud.logging.Client()


def create_app():
    app = Flask(__name__)

    db = MongoDBClient(f"mongodb+srv://{os.getenv('MONGO_NAME')}:{os.getenv('MONGO_PASS')}@feedbacks.azzoqaj.mongodb.net/?retryWrites=true&w=majority")

    def generate_response(data, code):
        return app.response_class(
            response=json.dumps(data),
            status=code,
            mimetype='application/json'
        )

    def generate_success_response():
        return generate_response({"success": True}, 200)


    def parse_and_count(feedbacks):
        results = dict()
        for f in feedbacks:
            model = f.get('model', 'unknown')
            if model not in results:
                results[model] = {'goods': 0, 'bads': 0}

            feedback = f.get('feedback', None)
            if feedback == 'good':
                results[model]['goods'] += 1
            elif feedback == 'bad':
                results[model]['bads'] += 1

        return results


    # For debugging
    @app.route('/ping', methods=['GET'])
    def ping_pong():
        return generate_response({'message': 'Pong!'}, 200)


    @app.route('/feedback/detection', methods=['POST'])
    def evaluate_detection():
        model = request.json.get('model')
        if not model:
            return generate_response({"message": "model is not included in the body."}, 400)

        is_positive = request.json.get('is_positive', None)
        if is_positive is None:
            return generate_response({"message": "is_positive is not included in the body."}, 400)

        db.put_feedback("detection", model, "good" if is_positive else "bad")

        return generate_success_response()


    @app.route('/feedback/detection', methods=['GET'])
    def get_detection_feedbacks():
        feedbacks = db.get_feedbacks("detection")
        return generate_response(parse_and_count(feedbacks), 200)


    @app.route('/feedback/translation', methods=['POST'])
    def evaluate_translation():
        model = request.json.get('model')
        if not model:
            return generate_response({"message": "model is not included in the body."}, 400)

        is_positive = request.json.get('is_positive', None)
        if is_positive is None:
            return generate_response({"message": "is_positive is not included in the body."}, 400)

        db.put_feedback("translation", model, "good" if is_positive else "bad")

        return generate_success_response()


    @app.route('/feedback/translation', methods=['GET'])
    def get_translation_feedbacks():
        feedbacks = db.get_feedbacks("translation")
        return generate_response(parse_and_count(feedbacks), 200)


    @app.route('/feedback/description', methods=['POST'])
    def evaluate_description():
        model = request.json.get('model')
        if not model:
            return generate_response({"message": "model is not included in the body."}, 400)

        is_positive = request.json.get('is_positive', None)
        if is_positive is None:
            return generate_response({"message": "is_positive is not included in the body."}, 400)

        db.put_feedback("description", model, "good" if is_positive else "bad")

        return generate_success_response()


    @app.route('/feedback/description', methods=['GET'])
    def get_description_feedbacks():
        # looks like [{"timestamp": "2011-10-05T14:48:00.000Z", "service": "description", "model": "chatgpt", "feedback": "good"}, ...]
        feedbacks = db.get_feedbacks("description")
        return generate_response(parse_and_count(feedbacks), 200)


    return app





# Run it with "python3 main.py"
if __name__ == "__main__":
    app = create_app()
    app.run(host='0.0.0.0', port=8080)
