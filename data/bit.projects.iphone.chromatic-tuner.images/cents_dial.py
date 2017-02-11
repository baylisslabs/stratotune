import sys
import math
from svgwrite import Point,Rect,SvgWriter


_fontStyle = 'stroke: none; fill: black; font-family: helvetica; font-size: 18pt;'

_arcStyle = 'stroke: black; stroke-width: 4; fill: none'

_majorTickLength = 30
_majorTickStyle = 'stroke: black; stroke-width: 3'

_minorTickLength = 15
_minorTickStyle = 'stroke: black; stroke-width: 1'

_midTickLength = 20
_midTickStyle = 'stroke: black; stroke-width: 2'

_notesTopMargin = 7
 
_width = 300
_height = 140

_outerRadius = _width
_semiToneRadians = 0.6*math.pi
_centsPerSemi = 100

_viewBox = Rect(-_width/2,-_width-_height/2,_width,_height)
sw = SvgWriter(_width,_height,_viewBox)



def cents_to_rads(cents):
    return cents * _semiToneRadians / _centsPerSemi + math.pi / 2        

def dial_arc(writer,start):
    #limit = 25.2;
    limit = 18.0;
    h=125.0
    min_alpha = cents_to_rads(-limit)
    max_alpha = cents_to_rads(limit)   
    if start:
        sw.move_to(Point.FromPolar(_outerRadius+h,min_alpha).Translate(0,h))
    else:
        sw.line_to(Point.FromPolar(_outerRadius+h,min_alpha).Translate(0,h))
    sw.arc_to(to=Point.FromPolar(_outerRadius+h,max_alpha).Translate(0,h),rx=_outerRadius+h, ry=_outerRadius+h)
    
dial_arc(sw,True)    
sw.stroke_path(style=_arcStyle)


sw.move_to(Point(_viewBox.right(),_viewBox.top()))
sw.line_to(Point(_viewBox.right(),_viewBox.bottom()))
dial_arc(sw,False)  
sw.line_to(Point(_viewBox.left(),_viewBox.bottom()))
sw.line_to(Point(_viewBox.left(),_viewBox.top()))
sw.close_path()
sw.define_clip_path(path_id='dial-arc',style=_arcStyle)
#sw.stroke_path(style=_arcStyle)

for i in range(-25,26):
    theta = cents_to_rads(i)
    radialU = Point.FromPolar(1,theta)
    if i%10==0:
        ticklen = _majorTickLength
        tickstyle = _majorTickStyle
    elif i%5==0:
        ticklen = _midTickLength
        tickstyle = _midTickStyle
    else:
        ticklen = _minorTickLength
        tickstyle = _minorTickStyle
        
    if abs(i)<20 or i%5==0:    
        inner = radialU.Scale(_outerRadius)
        outer = radialU.Scale(_outerRadius+ticklen)
        sw.add_line(a=outer,b=inner)
        sw.stroke_path(style=tickstyle+'; clip-path:url(#dial-arc);')
          
for i in range(-2,3):
    theta = cents_to_rads(i*10)
    radialU = Point.FromPolar(1,theta)  
    text = str(abs(i*10))
    if i<0:
        text = '+'+text
    elif i > 0:
        text = '-'+text
            
    outer = radialU.Scale(_outerRadius + _majorTickLength + _notesTopMargin);
    point = Point(outer.x, outer.y); 
    sw.add_text(point,text,_fontStyle+' text-anchor:middle;')
    

sw.add_text(Point(_viewBox.right()-25,_viewBox.top()+28),u"\u266f",_fontStyle+' text-anchor:end;')
sw.add_text(Point(_viewBox.left()+25,_viewBox.top()+28),u"\u266d",_fontStyle)
                
  
      
sw.write_xml(sys.stdout)  
#sw.write_xml(sys.stderr)  

