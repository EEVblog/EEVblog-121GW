using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App_112GW
{
    public delegate void TimerCallback();
    public delegate void TimerCleanup();

    class EnhancedTimerCallback
    {
        private TimerCallback   mCallback;
        private TimerCleanup    mCleanup;

        private bool            mRunning;
        public  bool            Finished;

        public EnhancedTimerCallback(TimeSpan pPeriod, TimerCallback pCallback, TimerCleanup pCleanup)
        {
            mCallback = pCallback;
            mCleanup = pCleanup;

            mRunning = true;
            Finished = false;

            Device.StartTimer(pPeriod, Execute);
        }
        private bool Execute()
        {
            //If the timer is running run the callback function.
            if (mRunning)
                mCallback();

            //Indicate timer is finished and schedule cleanup
            Finished = true;
            mCleanup();

            //Stop timer
            return mRunning = false;
        }
        public void Stop()
        {
            mRunning = false;
        }
    }

    class EnhancedTimer
    {
        TimerCallback               mCallback;
        List<EnhancedTimerCallback> mCallbackSchedule;
        private TimeSpan            mPeriod;

        public EnhancedTimer(TimeSpan pPeriod, TimerCallback pCallback)
        {
            mPeriod     = pPeriod;
            mCallback   = pCallback;

            mCallbackSchedule = new List<EnhancedTimerCallback>();
        }

        private void Cleanup()
        {
            //Remove finished items
            int i = 0;
            while ((mCallbackSchedule.Count > 0) && (i < mCallbackSchedule.Count))
            {
                if (mCallbackSchedule[i].Finished)
                    mCallbackSchedule.RemoveAt(i);

                i++;
            }
        }

        public void Start()
        {
            mCallbackSchedule.Add(new EnhancedTimerCallback(mPeriod, mCallback, Cleanup));
        }
        public void Stop()
        {
            foreach (EnhancedTimerCallback Callback in mCallbackSchedule)
                Callback.Stop();
        }
    }
}
