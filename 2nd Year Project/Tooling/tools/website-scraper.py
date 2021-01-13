from typing import Dict, List
from core.tool import Tool
from glob import glob
import re


class WebsiteScraper(Tool):
    url_delimiting_characters = r"\"/'<> "
    social_sites = ["facebook", "twitter"]

    def __init__(self, args: Dict[str, str], working_dir: str=""):
        super().__init__(args, working_dir)
        self.in_files = args["in"]

        re_sites = "|".join(self.social_sites)
        self.re_pattern = re.compile(r"(?i)\s*https://www\.(?:" + re_sites + r")\.com/[a-z]*[" + self.url_delimiting_characters + r"]")
        self.link_blacklist = ["https://www.facebook.com",
                               "https://www.facebook.com/sharer",
                               "https://www.facebook.com/pages",
                               "https://www.facebook.com/groups",
                               "https://www.facebook.com/plugins",
                               "https://www.facebook.com/pg",
                               "https://www.facebook.com/WordPresscom",
                               "https://www.twitter.com"]

    def run(self, _: List[Dict]) -> List[Dict]:
        data = []
        for file_name in glob(self.in_files):
            school_id = file_name.rstrip(".html")
            # Some files can have odd chars (0x92 and 0x9d specifically) so try opening with different decoders
            contents = None
            try:
                with open(file_name, encoding="utf8") as file:
                    contents = file.read()
            except UnicodeDecodeError:
                # UnicodeDecodeError happens when the expected encoding isn't used
                try:
                    with open(file_name, encoding="cp1252") as file:
                        contents = file.read()
                except UnicodeDecodeError:
                    pass
                except OSError:
                    pass
            except OSError:
                # OSError comes when a file is re/moved between being found and being opened
                pass
            if contents is not None:
                links = self.get_links_from(contents)
                if len(links) > 0:
                    data.append({"URN": school_id, **links})
        return data

    def get_links_from(self, file_contents: str) -> Dict:
        found_links = re.findall(self.re_pattern, file_contents)
        # some schools have twitter feeds on their pages, meaning multiple links come up
        # a lot of these links will be mentions and not the school, so count how many times
        # a link appears and take the most popular
        links = {}
        for link in found_links:
            link = link.rstrip(self.url_delimiting_characters).lstrip(self.url_delimiting_characters)
            if link not in self.link_blacklist:
                if link in links.keys():
                    links[link] += 1
                else:
                    links[link] = 1
        # sort the links into pairs of items with their counts, sort them and reverse the list into descending
        link_pairs = sorted(links.items(), key=lambda key_value: (key_value[1], key_value[0]))[::-1]
        links = [link_pair[0] for link_pair in link_pairs]
        social_links = {}
        for social_site in self.social_sites:
            for social_link in links:
                if social_site in social_link:
                    social_links[social_site.title() + "Link"] = social_link
                    break

        return social_links


entry = WebsiteScraper
