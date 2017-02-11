import sys
import math
from svgwrite import Point,Rect,SvgWriter

 
_fontSize = 32;
_fontStyle = 'stroke: none; fill: black; font-family: helvetica; font-size: %dpt;' % _fontSize

_arcStyle = 'stroke: black; stroke-width: 4; fill: none'

_majorTickLength = 20
_majorTickStyle = 'stroke: black; stroke-width: 4'

_minorTickLength = 10
_minorTickStyle = 'stroke: black; stroke-width: 2'

_midTickLength = 15
_midTickStyle = 'stroke: black; stroke-width: 2'

_notesTopMargin = 7
_topMargin = 5
_semiTonePts = 100
_semiTones = 12;
_octavePts = _semiTonePts * _semiTones
_octaves = 12  
_midOctave = 5

_width = _octaves*_octavePts
_height = 75

_viewBox = Rect(-_width/2,0,_width,_height)
sw = SvgWriter(_width,_height,_viewBox)

 
def subscript(number):
    text = str(number)
    out = u''
    digits = u"\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089"
    for c in text:
        if str.isdigit(c):
           out += digits[ord(c)-ord('0')]
        elif c=='-':
            out += u'\u208b'
            
    return out

def write_scale_tape(note_lookup):
    for i in range(-_octaves*_semiTones*10,_octaves*_semiTones*10+1):   
        x = i*_semiTonePts/10
        
        if i%10==0:
            ticklen = _majorTickLength
            tickstyle = _majorTickStyle
        elif i%5==0:
            ticklen = _midTickLength
            tickstyle = _midTickStyle
        else:
            ticklen = _minorTickLength
            tickstyle = _minorTickStyle
               
        top = Point(x,_topMargin)
        bottom = top.Translate(0,ticklen)
        sw.add_line(a=top,b=bottom)
        sw.stroke_path(style=tickstyle)
          
    for i in range(-_octaves*_semiTones,_octaves*_semiTones+1):   
        x = i*_semiTonePts
        octave = i/12+_midOctave
        text = note_lookup[i%12] + subscript(octave)
        point = Point(x,_topMargin+_majorTickLength + _notesTopMargin + _fontSize);    
        sw.add_text(point,text,_fontStyle+' text-anchor:middle;')
        
          
    sw.write_xml(sys.stdout)  
#sw.write_xml(sys.stderr)  

