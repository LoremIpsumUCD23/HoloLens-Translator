# docker buildx build --platform linux/amd64 -t sattttto/ml-server:0.0.1 . --push
FROM python:3.9-slim

# ENV
ENV PYTHONUNBUFFERED True
ENV PORT 8080
ENV ENV gcp

# Copy local code to the container image.
WORKDIR /app
COPY . ./

# Install production dependencies.
RUN pip install --no-cache-dir -r requirements.txt

# Run the server
CMD exec gunicorn --bind :8080 --workers 1 --threads 8 "main:create_app()"
