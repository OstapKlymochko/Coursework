from typing import Type, Dict

class MessageHandler:
    registry: Dict[str, Type["MessageHandler"]] = {}

    def __init__(self):
        pass #nothing to init

    @classmethod
    def register(cls, handler_cls: Type["MessageHandler"]) -> None:
        if not hasattr(handler_cls, "message_type"):
            raise ValueError(f"{handler_cls.__name__} must define a `message_type` attribute")
        cls.registry[handler_cls.message_type] = handler_cls

    async def handle(self, payload: dict):
        raise NotImplementedError