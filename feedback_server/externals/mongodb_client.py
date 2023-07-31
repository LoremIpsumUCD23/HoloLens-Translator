from pymongo import MongoClient
from datetime import datetime

class MongoDBClient(object):
    _instance = None
    _feedback_coll = None

    def __new__(cls, conn: str):
        if cls._instance is None:
            cls._instance = super(MongoDBClient, cls).__new__(cls)
            # init connection
            cls._client = MongoClient(conn)
            try:
                cls._client.admin.command('ping')
                print("Successfully connected to MongoDB!")
            except Exception as e:
                print(e)
                return cls._instance

            # create a collection "feedbacks" in the database "Feedbacks"
            db = cls._client["Feedbacks"]
            cls._feedback_coll = db["feedbacks"]

        return cls._instance

    def put_feedback(self, service: str, model: str, feedback: str):
        self._feedback_coll.insert_one({
            "timestamp": datetime.now().isoformat(),
            "service": service,
            "model": model,
            "feedback": feedback
        })

    def get_feedbacks(self, service: str):
        pipeline = [{"$match": {"service": service}}]
        results = self._feedback_coll.aggregate(pipeline)
        return [doc for doc in results]
