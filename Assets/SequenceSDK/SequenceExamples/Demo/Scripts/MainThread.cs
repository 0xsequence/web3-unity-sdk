using System;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceDemo
{
    internal class MainThread : MonoBehaviour
    {
        internal static MainThread mainThread;
        Queue<Action> jobs = new Queue<Action>();

        void Awake()
        {
            mainThread = this;
        }

        void Update()
        {
            while (jobs.Count > 0)
                jobs.Dequeue().Invoke();
        }

        internal void AddJob(Action newJob)
        {
            jobs.Enqueue(newJob);
        }
    }
}
