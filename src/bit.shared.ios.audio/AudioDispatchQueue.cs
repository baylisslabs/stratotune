using System;
using System.Collections.Generic;

using bit.shared.logging;

namespace bit.shared.ios.audio
{
    // Goals: thread-safe
    //    dequeue: pop events in-order pushed
    //    superseeding: events with same id override earlier events and remove them from queue      
    public class AudioDispatchQueue
    {
        static Logger _log = LogManager.GetLogger("AudioDispatchQueue");

        private struct QEntry
        {
            public int msgId;                   
            public bool isSuperSeedable;
            public Action action; 
        }
               
        private object _lock = new object();
        private int _maxLength;
        private LinkedList<QEntry> _dispatchQueue;
        private Dictionary<int,LinkedListNode<QEntry>> _idIndex;

        public AudioDispatchQueue (int maxLength)
        {
            _maxLength = maxLength;
            _dispatchQueue = new LinkedList<QEntry>();
            _idIndex = new Dictionary<int, LinkedListNode<QEntry>>();
        }

        public void Push (int msgId, bool isSuperSeedable, Action action)
        {
            bool discarded = false;

            lock (_lock) {
                if (_idIndex.ContainsKey (msgId)) {
                    _dispatchQueue.Remove (_idIndex [msgId]);
                    _idIndex.Remove (msgId);
                }

                if (_dispatchQueue.Count < _maxLength) {
                    var node = _dispatchQueue.AddLast (new QEntry { msgId = msgId, isSuperSeedable = isSuperSeedable, action = action });
                    if (isSuperSeedable) {
                        _idIndex [msgId] = node;
                    } 
                } else {
                    discarded = true;
                }
            }

            if (discarded) {
                _log.Warn ("max queue length {0} reached. Action discarded", _maxLength);
            }
        }

        public Action Pop ()
        {
            lock (_lock) {               
                var node = _dispatchQueue.First;
                if(node!=null) {
                    _dispatchQueue.Remove(node);
                    if(node.Value.isSuperSeedable) {
                        _idIndex.Remove(node.Value.msgId);
                    }
                    return node.Value.action;               
                }
            }
            return null;
        }

        public bool ExecuteNext ()
        {
            Action action = this.Pop ();
            if (action!=null) {
                try {
                    action();
                } catch (Exception ex) {
                    _log.Error("action failed",ex);
#if DEBUG
                    throw;
#endif  
                }
                return true;
            }
            return false;
        }
    }
}

