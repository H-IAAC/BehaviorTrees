import re
import os

try:
    import cffconvert
except:
    import warnings
    warnings.warn("cffconvert not installed. Not updating README citation.")
    exit()

if os.path.isfile("CITATION.CFF"):
    cff_file = "CITATION.CFF"
elif os.path.isfile("CITATION.cff"):
    cff_file = "CITATION.cff"
else:
    import warnings
    warnings.warn("CITATION.cff file not found. Add file and reexecute.")
    exit()

with open(cff_file, "r") as file:
    cff_content = file.read()
    citation = cffconvert.Citation(cff_content)

citation_str = citation.as_bibtex(reference="my_citation")
citation_str = citation_str.replace("@misc", "@software")

citation_str = ("<!--CITATION START-->\n"+
                "```bibtex\n"+
                citation_str+
                "```\n"+
                "<!--CITATION END-->")

with open("README.md", "r") as file: 
    readme_text = file.read()

readme_text = re.sub(r"<!--CITATION START-->((.|\n)*)<!--CITATION END-->", citation_str, readme_text)

with open("README.md", "w") as file:
    file.write(readme_text)
