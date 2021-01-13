from typing import List, Dict
from importlib import import_module
from inspect import isclass
from abc import ABC, abstractmethod
from os import getcwd, chdir


class Tool(ABC):
    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        self._args = args
        self.working_dir = working_dir

    @abstractmethod
    def run(self, data: List[Dict]) -> List[Dict]:
        pass


def load(entry: str, args: Dict[str, str], working_dir: str = "") -> Tool:
    try:
        module = import_module(f'tools.{entry}')
        if hasattr(module, "entry"):
            if isclass(module.entry):
                if issubclass(module.entry, Tool):
                    starting_dir = getcwd()
                    chdir(working_dir)
                    try:
                        tool = module.entry(args, working_dir)
                    finally:
                        chdir(starting_dir)
                    return tool
                else:
                    raise RuntimeError(f'tool "{entry}" defines an entry point that is not '
                                       f'a subclass of {Tool.__name__}')
            else:
                raise RuntimeError(f'tool "{entry}" defines an entry point that is not a class')
        else:
            raise RuntimeError(f'tool "{entry}" does not define an entry point')
    except ImportError as e:
        raise RuntimeError(f'could not import tool "{entry}"', e)
