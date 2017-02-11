#!/usr/bin/python

"""
This script will convert the svg source to png @2x scale,
then post-process with the imagemagick convert "script" via the <im:convert> tag embedded within the svg.
The script is parsed into tokens and submitted as command line args to convert - [args*] -
"""

import sys
import os
import subprocess
import argparse
import xml.etree.ElementTree as et

XMLNS_SVG='{http://www.w3.org/2000/svg}'
XMLNS_IM='{http://www.bayliss-it.com.au/2012/imageproc}'

parser = argparse.ArgumentParser(description = "convert the svg source to png @2x scale")
parser.add_argument("-c", dest="source",help="SVG source file", metavar="FILE", required=True)
parser.add_argument("-o", dest="output",help="PNG output file @2x scale", metavar="FILE", required=True)
parser.add_argument("-v", dest="verbose",help="verbose output")

args = parser.parse_args()
    
svg_file_name = args.source
png_file_name = args.output

class StringToArgs:
    def __init__(self):
        self.args = []
        self.token = None
        self.in_quoted_str = None
        
    def put(self,c):        
        if self.token:
            self.token += c
        else:
            self.token = c
            
    def pushToken(self):
        if self.token:
            self.args.append(self.token)
            self.token = None
    
    def processChar(self,c):
        if self.in_quoted_str:
            if self.in_quoted_str==c:
                self.in_quoted_str=None
            elif c == "\n":
                raise Exception("unterminated string encountered")
            else:
                 self.put(c)
        elif c.isspace():
            self.pushToken()
        elif c ==  "'" or c == '"':           
            self.in_quoted_str=c                               
        else:
            self.put(c)
            
    def processString(self,data):
        for c in data:
            self.processChar(c)
        self.pushToken()
        return self.args
                
                                            
def call_checked_pipe(args,input_data):
    p1 = subprocess.Popen(args,stdin=subprocess.PIPE,stdout=subprocess.PIPE)
    (p1_out,p1_err) = p1.communicate(input_data)
    if p1_err:
        print p1_err
    if p1.returncode!=0:        
        sys.stderr.write('popen(%s)\n'%args)
        raise subprocess.CalledProcessError(p1.returncode,args[0],None)
    return p1_out

with open(svg_file_name) as svg_file:
    svg_data = svg_file.read()
    svg_data_2x = svg_data.replace('.png"','@2x.png"')
       
    png_data_2x_2 = png_data_2x_1 = call_checked_pipe(["/usr/bin/rsvg-convert","-z","2"],svg_data_2x)
    
    root = et.fromstring(svg_data_2x)    
    if root.tag==XMLNS_SVG+'svg':
        im_convert_elems = root.findall(XMLNS_IM+'convert')
        if len(im_convert_elems)>1:
            raise Exception("only one <convert> element allowed")
        elif len(im_convert_elems)==1:
            im_data = im_convert_elems[0].text
            im_data_2x = im_data.replace('.png','@2x.png')
            argsProc = StringToArgs()              
            im_args_data = argsProc.processString(im_data_2x)
            convert_args = ["/usr/bin/convert","-"]
            convert_args.extend(im_args_data)
            convert_args.append("-")
            png_data_2x_2 = call_checked_pipe(convert_args,png_data_2x_1)            
    else:
        raise Exception(XMLNS_SVG+"svg root element expected")      
    
    with open(png_file_name,"w") as png_file:
        png_file.write(png_data_2x_2)
    
    