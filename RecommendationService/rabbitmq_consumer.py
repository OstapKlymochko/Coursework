import aio_pika
import asyncio
import json
import re

import Consumers
from config import RABBITMQ_URL

QUEUE_PREFIX = 'recs-consumer'


async def consume():
    connection = await aio_pika.connect_robust(RABBITMQ_URL)
    channel = await connection.channel()

    for message_type, handler_cls in Consumers.MessageHandler.registry.items():

        exchange = await channel.declare_exchange(message_type, aio_pika.ExchangeType.FANOUT, durable=True)

        queue_name = re.sub('([A-Z]+)', r'-\1', message_type.split(':')[-1]).lower()
        queue_name = QUEUE_PREFIX + queue_name
        queue = await channel.declare_queue(queue_name, durable=True)

        await queue.bind(exchange)

        await queue.consume(lambda x, y=handler_cls: on_message(x, y))
        print(
            f"[RabbitMQ] Listening for {message_type} messages on queue '{queue_name}'")

    await asyncio.Future()


async def on_message(message: aio_pika.IncomingMessage, handler_cls: Consumers.MessageHandler):
    async with message.process():
        try:
            payload = json.loads(message.body.decode())['message']
            await handler_cls().handle(payload)
        except Exception as e:
            print(
                f"[Error] Handling {handler_cls.__name__} message: {e}")
            raise e
