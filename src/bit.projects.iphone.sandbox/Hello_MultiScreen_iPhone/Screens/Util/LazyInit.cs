using System;

namespace Hello_MultiScreen_iPhone
{
	public class LazyInit<T> where T : new()
	{
		private Func<T> _creator;
		private T _instance;

		public LazyInit (Func<T> creator)
		{
			_creator = creator;
		}

		public LazyInit ()
		{
			_creator = ()=>(new T());
		}

		public T Obj {
			get {
				if (_instance == null) {
					_instance = _creator ();
				}
				return _instance;
			}
		}

		public void IfNotNull(Action<T> action) {
			if(_instance!=null) {
				action(_instance);
			}
		}
	}
}

