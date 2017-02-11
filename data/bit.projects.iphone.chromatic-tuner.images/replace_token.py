#!/usr/bin/python

"""
This script will replace the token argument(argv[2])
in the input file (argv[1]) with 
text read fromn stdin.
"""

import sys

template_file_name = sys.argv[1]
token = sys.argv[2]

with open(template_file_name) as template_file:
    template_str = template_file.read()
    input_data = sys.stdin.read()    
    sys.stdout.write(template_str.replace(token,input_data))
