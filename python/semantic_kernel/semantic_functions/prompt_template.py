# Copyright (c) Microsoft. All rights reserved.

import logging
from typing import TYPE_CHECKING, Any, List, Optional

from semantic_kernel.plugin_definition.parameter_view import ParameterView
from semantic_kernel.semantic_functions.prompt_template_base import PromptTemplateBase
from semantic_kernel.semantic_functions.prompt_template_config import (
    PromptTemplateConfig,
)
from semantic_kernel.template_engine.blocks.block_types import BlockTypes
from semantic_kernel.template_engine.blocks.var_block import VarBlock
from semantic_kernel.template_engine.protocols.prompt_templating_engine import (
    PromptTemplatingEngine,
)

if TYPE_CHECKING:
    from semantic_kernel.orchestration.kernel_context import KernelContext

logger: logging.Logger = logging.getLogger(__name__)


class PromptTemplate(PromptTemplateBase):
    template: str
    template_engine: PromptTemplatingEngine
    prompt_config: PromptTemplateConfig

    def __init__(
        self,
        template: str,
        template_engine: PromptTemplatingEngine,
        prompt_config: PromptTemplateConfig,
        log: Optional[Any] = None,
    ) -> None:
        if log:
            logger.warning("The `log` parameter is deprecated. Please use the `logging` module instead.")
        super().__init__(
            template=template,
            template_engine=template_engine,
            prompt_config=prompt_config,
        )

    def get_parameters(self) -> List[ParameterView]:
        seen = set()

        result = []
        for param in self.prompt_config.parameters:
            if param is None:
                continue

            result.append(
                ParameterView(
                    name=param.name,
                    description=param.description,
                    default_value=param.default_value,
                    type_=param.type_,
                    required=param.required,
                )
            )

            seen.add(param.name)

        blocks = self.template_engine.extract_blocks(self.template)
        for block in blocks:
            if block.type != BlockTypes.VARIABLE:
                continue
            if block is None:
                continue

            var_block: VarBlock = block  # type: ignore
            if var_block.name in seen:
                continue

            result.append(ParameterView(name=var_block.name, description="", default_value=""))

            seen.add(var_block.name)

        return result

    async def render(self, context: "KernelContext") -> str:
        return await self.template_engine.render(self.template, context)
