FROM pytorch/pytorch

# Install Transformers and Flask libraries
RUN pip install transformers Flask
RUN pip install sentencepiece


# If you have a custom model, copy the model files (adjust the path if needed)
# COPY my_marian_model /app/my_marian_model

# Copy the script to run the server from the correct location
COPY ml_server/translation_separate.py /app/translation_separate.py

# Set the working directory
WORKDIR /app

# Run the server
CMD ["python", "translation_separate.py"]


