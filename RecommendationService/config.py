from dotenv import load_dotenv
from os import getenv

# load_dotenv()

RABBITMQ_URL = getenv('RABBITMQ_URL')  
DATABASE_URL = getenv('DATABASE_URL')
S3_CLIENT_ID = getenv('S3_CLIENT_ID')
S3_SECRET_KEY = getenv('S3_SECRET_KEY')
S3_BUCKET_NAME = getenv('S3_BUCKET_NAME')
S3_REGION = getenv('S3_REGION')
MODEL_PATH = getenv('MODEL_PATH')