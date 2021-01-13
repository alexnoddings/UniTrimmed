from typing import Dict, List
from core.tool import Tool, load as load_tool
from os import getcwd, chdir
from os import path


class Process:
    def __init__(self, process_id: str, tools: List[Tool]):
        self.id = process_id
        self._tools = tools

    def run(self):
        starting_dir = getcwd()
        data = []
        try:
            for tool in self._tools:
                chdir(path.join(starting_dir, tool.working_dir))
                data = tool.run(data)
        finally:
            chdir(starting_dir)


def load(process_id: str, tools_data: List[Dict]) -> Process:
    tools = []
    for tool_data in tools_data:
        tool = load_tool(entry=tool_data["entry"],
                         args={k: v for args in tool_data["args"] for k, v in args.items()} if "args" in tool_data.keys() else {},
                         working_dir=tool_data["dir"] if "dir" in tool_data.keys() else ".")
        tools.append(tool)
    return Process(process_id, tools)
