from typing import Dict, List
from json import loads
from core.tool import Tool


class Translator(Tool):
    key_translations = {}
    key_value_translations = {}
    value_translations = {}

    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        super().__init__(args, working_dir)
        self.load_translations(args["cfg"])

    def run(self, data: List[Dict]) -> List[Dict]:
        for school in data:
            # copy the keys to avoid it getting changed during iteration
            keys = [k for k in school.keys()]
            # Key Translations
            for key in keys:
                if key in self.key_translations.keys():
                    value = school[key]
                    del school[key]
                    school[self.key_translations[key]] = value
            # Key Value Translations
            keys = [k for k in school.keys()]
            for key, value in school.items():
                if key in self.key_value_translations.keys():
                    if value in self.key_value_translations[key].keys():
                        school[key] = self.key_value_translations[key][value]
            # Value Translations
            for key in keys:
                value = school[key]
                if value in self.value_translations.keys():
                    school[key] = self.value_translations[value]
        return data

    def load_translations(self, cfg_path: str):
        with open(cfg_path) as cfg_file:
            cfg = cfg_file.read()
        data = loads(cfg)
        self.key_translations = data["key_translations"]
        self.key_value_translations = data["key_value_translations"]
        self.value_translations = data["value_translations"]


entry = Translator
