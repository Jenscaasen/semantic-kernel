# Copyright (c) Microsoft. All rights reserved.
from typing import Optional
from xml.etree import ElementTree
from xml.etree.ElementTree import Element

from semantic_kernel.contents.chat_role import ChatRole
from semantic_kernel.contents.kernel_content import KernelContent


class ChatMessageContent(KernelContent):
    """This is the base class for chat message response content.

    All Chat Completion Services should return a instance of this class as response.
    Or they can implement their own subclass of this class and return an instance.

    Args:
        inner_content: Optional[Any] - The inner content of the response,
            this should hold all the information from the response so even
            when not creating a subclass a developer can leverage the full thing.
        ai_model_id: Optional[str] - The id of the AI model that generated this response.
        metadata: Dict[str, Any] - Any metadata that should be attached to the response.
        role: ChatRole - The role of the chat message.
        content: Optional[str] - The text of the response.
        encoding: Optional[str] - The encoding of the text.

    Methods:
        __str__: Returns the content of the response.
    """

    role: ChatRole
    content: Optional[str] = None
    encoding: Optional[str] = None

    def __str__(self) -> str:
        return self.content or ""

    def to_prompt(self, root_key: str) -> str:
        """Convert the ChatMessageContent to a prompt.

        Returns:
            str - The prompt from the ChatMessageContent.
        """

        root = Element(root_key)
        root.set("role", self.role.value)
        root.text = self.content or ""
        return ElementTree.tostring(root, encoding=self.encoding or "unicode")

    @classmethod
    def from_element(cls, element: Element) -> "ChatMessageContent":
        """Create a new instance of ChatMessageContent from a prompt.

        Args:
            prompt: str - The prompt to create the ChatMessageContent from.

        Returns:
            ChatMessageContent - The new instance of ChatMessageContent.
        """
        args = {"role": element.get("role", ChatRole.USER.value), "content": element.text}
        return cls(**args)
