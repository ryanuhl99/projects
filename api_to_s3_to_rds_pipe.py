import logging
import pandas as pd
import requests
import urllib
from io import StringIO
from sqlalchemy import create_engine
import boto3
import os


# logging config
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s | %(levelname)s | [%(filename)s:%(lineno)d] %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S'
)

# import environmental vars
aws_access_key_id = os.getenv('AWS_ACCESS_KEY_ID')
aws_secret_access_key = os.getenv('AWS_SECRET_ACCESS_KEY')
username = os.getenv('DB_USERNAME')
password = os.getenv('DB_PASSWORD')
server = os.getenv('SERVER')

# instantiate boto client
s3 = boto3.client(
    's3',
    region_name='us-west-1',
    aws_access_key_id=aws_access_key_id,
    aws_secret_access_key=aws_secret_access_key
)


def extract_from_api():

    try:
        url = 'https://api.covidtracking.com/v1/states/info.csv'
        response = requests.get(url)
        response.raise_for_status()
        s = StringIO(response.text) #loading csv into string buffer to read as df
        df_s = pd.read_csv(s)
        string_buffer = StringIO() #creating empty string buffer to load df into
        df_s.to_csv(string_buffer, index=False)
        string_buffer.seek(0) #moving cursor to beginning of string
        return string_buffer.getvalue() #we do this so we can load the obj from memory into s3 without having to save to disk
    except Exception as e:
        logging.error(f'Error extracting data from api: {e}')
        raise


def s3_load(file, bucket='covid-data-202405'):

    try:
        name = 'covid_states_metadata'

        s3.put_object(      #loading byte-like obj into s3 from memory
            Bucket=bucket,
            Key=f'{name}.csv',
            Body=file
        )
        logging.info(f'Successfully uploaded {name} into {bucket}')
    except Exception as e:
        logging.error(f'Error uploading to s3 bucket: {e}')
        raise


def extract_from_s3(bucket='covid-data-202405', key='covid_states_metadata.csv'):

    try:
        response = s3.get_object(
                    Bucket=bucket,
                    Key=key
        )
        logging.info(f'Successfully extracted {key} from {bucket}')
        return response['Body'].read().decode('utf-8') #isolating content from the s3 metadata and coding it correctly
    except Exception as e:
        logging.error(f'Error extracting file from s3 bucket: {e}')
        raise


def rds_engine(server, username, password, database='covid'):

    try:
        params = urllib.parse.quote_plus(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};'
            f'SERVER={server};'
            f'DATABASE={database};'
            f'UID={username};'
            f'PWD={password}'
        )
        connection_string = f'mssql+pyodbc:///?odbc_connect={params}'
        engine = create_engine(connection_string)
        logging.info('Successfully connected to the rds mssql server')
        return engine
    except Exception as e:
        logging.error(f'Error connecting to the rds mssql server {e}')
        raise


def main():

    try:

        covid_csv = extract_from_api() # E

        s3_load(covid_csv) # L

        s3_csv = extract_from_s3() # E

        s3_df = pd.read_csv(StringIO(s3_csv)) # T

        engine = rds_engine(server, username, password)

        s3_df.to_sql('covid_state_metadata', con=engine, if_exists='append', index=False) # L
        logging.info('Successfully loaded csv into rds')

        engine.dispose()
        logging.info('sqlalchemy engine successfully disposed')

    except Exception as e:
        logging.error(f'error occurred in main func: {e}')


if __name__ == '__main__':
    main()