import sys
import math
import random

from svgwrite import Point,Rect,SvgWriter

_size = 128
sw = SvgWriter(_size,_size)
sw.add_imconvert('-channel RGBA -blur 1x1')

random.seed(17171717)
r = _size/2;
for j in range(0,(int)(r/2)):
    i=j*2
    sw.move_to(Point(r,r-i))
    sw.arc_to(Point(r,r+i), i, i)
    sw.arc_to(Point(r,r-i), i, i)
    sw.close_path()
    x = random.randint(170,230)
    sw.stroke_path(style="stroke: rgb(%d,%d,%d); stroke-width: 1; fill:none"%(x,x,x))
          
sw.write_xml(sys.stdout)  
#sw.write_xml(sys.stderr)  

