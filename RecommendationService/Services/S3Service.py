import boto3
from botocore.exceptions import ClientError
from config import S3_CLIENT_ID, S3_SECRET_KEY, S3_BUCKET_NAME, S3_REGION


class S3Service:
    def __init__(self):
        self.bucket_name = S3_BUCKET_NAME
        self.s3_client = boto3.client(
            's3',
            aws_access_key_id=S3_CLIENT_ID,
            aws_secret_access_key=S3_SECRET_KEY,
            region_name=S3_REGION
        )

    def get_presigned_url(self, file_key: str, expires_in_days: int = 1):
        try:
            url = self.s3_client.generate_presigned_url(
                'get_object',
                Params={'Bucket': self.bucket_name, 'Key': file_key},
                ExpiresIn=expires_in_days * 24 * 60 * 60
            )
            return url
        except ClientError as e:
            print(f"Error generating presigned URL: {e}")
            return None
