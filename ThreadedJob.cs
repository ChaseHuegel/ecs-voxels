using System.Threading;
using System.Collections;

namespace Swordfish
{
	public class ThreadedJob : UnityEngine.ScriptableObject
	{
		public bool stop = false;
		public int updates = 0;

		private bool m_IsDone = false;
		private object m_Handle = new object();
		private Thread m_Thread = null;
		public bool IsDone
		{
			get
			{
				bool tmp;
				lock (m_Handle)
				{
					tmp = m_IsDone;
				}
				return tmp;
			}
			set
			{
				lock (m_Handle)
				{
					m_IsDone = value;
				}
			}
		}

		public virtual void Start()
		{
			stop = false;
			m_Thread = new Thread(Run);
			m_Thread.IsBackground = true;
			m_Thread.Start();
		}

		public virtual void Abort()
		{
			stop = true;
			if (m_Thread != null)
			{
				m_Thread.Abort();
			}
		}

		protected virtual void ThreadFunction() { }

		protected virtual void OnFinished() { }

		public virtual bool Update()
		{
			if (IsDone)
			{
				OnFinished();
				return true;
			}
			return false;
		}

		// public IEnumerator WaitFor()
		// {
		// 	while(!Update())
		// 	{
		// 		yield return null;
		// 	}
		// }

		private void Stop()
		{
			stop = true;
		}

		private void Run()
		{
			ThreadFunction();
			IsDone = true;
		}
	}
}