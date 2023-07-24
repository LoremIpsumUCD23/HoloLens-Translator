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

@app.put("/gpt")
async def root(word):
    return {"ChatGPT response"}

@app.put("/dictionary")
async def root(word):
    return {"Dictionary response"}

@app.put("/translation")
async def root(word):
    return {"Translator response"}

@app.put("/imageAnalysis")
async def root(image):
    return {"Image Analysis response"}

