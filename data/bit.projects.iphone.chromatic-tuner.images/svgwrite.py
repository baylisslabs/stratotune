import math
from xml.etree import ElementTree as et

class Rect:        
    def __init__(self,x,y,width,height):
        self.x = x
        self.y = y
        self.width = width
        self.height = height
        
    def top(self):
        return self.y
        
    def left(self):
        return self.x
        
    def right(self):
        return self.x+self.width
        
    def bottom(self):
        return self.y+self.height
        
                       
class Point:        
    def __init__(self,x,y):
        self.x = x
        self.y = y    
        
    def Scale(self,r):
        return Point(self.x*r,self.y*r)
        
    def Translate(self,x,y):
        return Point(self.x+x,self.y+y)
        
    @staticmethod    
    def FromPolar(r,theta):
        return Point(r*math.cos(theta),-r*math.sin(theta))

class Circle:
    def __init__(self,centre,radius):
        self.centre = centre
        self.radius = radius
          

class SvgWriter:               
    def __init__(self,width,height,viewBox=None):
        attribs = {
             'xmlns':'http://www.w3.org/2000/svg'
            ,'xmlns:xlink':'http://www.w3.org/1999/xlink'
            ,'xmlns:ev':'http://www.w3.org/2001/xml-events' 
            ,'xmlns:im':'http://www.bayliss-it.com.au/2012/imageproc'
            ,'width':'%d'%width
            ,'height':'%d'%height }     
                            
        if viewBox!=None:
            attribs['viewBox'] = '%f %f %f %f'%(viewBox.x,viewBox.y,viewBox.width,viewBox.height)
            
        self._doc = et.Element('svg',attribs)  
        self._path = ''          
            
            
    def doc(self):
        return self._doc
        
    def move_to(self, a):
        self._path += ' M %f %f' % (a.x,a.y)
        
    def add_line(self, a, b):
        self._path += ' M %f %f L %f %f' % (a.x,a.y,b.x,b.y)
        
    def line_to(self, b):
        self._path += ' L %f %f' % (b.x,b.y)
        
    def arc_to(self, to, rx, ry, x_axis_rot=0, large_arc_flag=0, sweep_flag=0):
        self._path += ' A%f,%f %f %d,%d %f,%f' % (rx,ry,x_axis_rot,large_arc_flag,sweep_flag,to.x,to.y) 
        
    def close_path(self):
        self._path += ' Z'
        
    def define_clip_path(self,path_id,style):
        elem = et.Element('clipPath', id=path_id)
        et.SubElement(elem, 'path', d=self._path.strip(), style=style)
        self._path = ''
        self._doc.append(elem) 
      
    def add_text(self, pos, text, style):       
        elem = et.Element('text', x=str(pos.x), y=str(pos.y), style=style)
        elem.text = text
        self._doc.append(elem)  
        
    def stroke_path(self,style):
        et.SubElement(self._doc, 'path', d=self._path.strip(), style=style)
        self._path = ''
        
    def add_imconvert(self,script):
        elem = et.Element('im:convert')
        elem.text = script
        self._doc.append(elem)  
        
    def write_xml(self,out_file):       
        out_file.write('<?xml version="1.0" encoding="utf-8"?>\n')
        out_file.write(et.tostring(self._doc))        
