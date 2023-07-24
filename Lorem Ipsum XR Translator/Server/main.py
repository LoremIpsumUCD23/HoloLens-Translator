from fastapi import FastAPI

# Run Server uvicorn server with
# python -m uvicorn main:app --reload

app = FastAPI()

@app.get("/")
async def root():
    return {"message": "Hello World"}

@app.get("/welcome")
async def root():
    return {"Hello Human! This server is up and running."}

@app.post("/gpt")
async def root(word):
    return {"ChatGPT response"}

@app.post("/dictionary")
async def root(word):
    return {"Dictionary response"}

@app.post("/translation")
async def root(word):
    return {"Translator response"}



