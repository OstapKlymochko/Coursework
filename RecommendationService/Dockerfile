FROM python:3.11-slim

WORKDIR /app

RUN apt-get update && apt-get install -y build-essential gcc ffmpeg && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .

RUN pip install -r requirements.txt 
RUN pip install debugpy

COPY . .

EXPOSE 5005

# CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "5005", "--reload"]
CMD ["python", "-Xfrozen_modules=off", "-m", "debugpy", "--listen", "0.0.0.0:5678", "--wait-for-client", "-m", "uvicorn", "main:app", "--host", "0.0.0.0", "--port", "5005", "--reload"]
