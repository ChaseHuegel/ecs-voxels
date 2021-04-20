using System.Threading;

namespace Swordfish {
namespace threading
{

public class ThreadWorker
{
	private volatile bool stop = false;
	private volatile bool pause = false;

	private Thread thread = null;

	public ThreadWorker()
	{
		thread = new Thread(Tick);
	}

	public void Start()
	{
		stop = false;
		pause = false;
		thread.Start();
	}

	public void Stop()
	{
		stop = true;
	}

	public void Restart()
	{
		stop = false;
		pause = false;
		thread.Abort();
		thread.Start();
	}

	public void Pause()
	{
		pause = true;
	}

	public void Unpause()
	{
		pause = false;
	}

	public void TogglePause()
	{
		pause = !pause;
	}

	public void Dispose()
	{
		thread.Abort();
	}

	private void Tick()
	{
		while (stop == false)
		{
			while (pause == false)
			{
				Execute();
			}

			Thread.Sleep(200);	//	Sleep when paused
		}

		//	Stopped thread safely
	}

	public virtual void Execute() {}
}

}
}