import sys
import math
import music_notation_type as mnt
from svgwrite import Point,Rect,SvgWriter

"""
Stylised representation for app-store icon
"""
 
_fontSize = 100;
_fontStyle = 'stroke: none; fill: black; font-family: helvetica; font-size: %dpt;' % _fontSize

_arcStyle = 'stroke: black; stroke-width: 4; fill: none'

_majorTickLength = 50
_majorTickStyle = 'stroke: black; stroke-width: 6'

_minorTickLength = 25
_minorTickStyle = 'stroke: black; stroke-width: 4'

_midTickLength = 37
_midTickStyle = 'stroke: black; stroke-width: 4'

_notesTopMargin = 7
_topMargin = 15
_semiTonePts = 200
_semiTones = 12;
_octavePts = _semiTonePts * _semiTones
_octaves = 1  
_midOctave = 4

_width = 460
_height = 200

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
        j = i+9
        octave = j/12+_midOctave
        text = note_lookup[j%12] + subscript(octave)
        point = Point(x,_topMargin+_majorTickLength + _notesTopMargin + _fontSize);    
        sw.add_text(point,text,_fontStyle+' text-anchor:middle;')
        
          
    sw.write_xml(sys.stdout)  

def main(): 
    write_scale_tape(mnt.music_notation_types['English'])    
    
if __name__ == "__main__":
    main()    


