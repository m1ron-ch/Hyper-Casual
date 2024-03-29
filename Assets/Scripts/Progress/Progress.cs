using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class Progress : MonoBehaviour
{
    [Serializable]
    public class Object
    {
        [JsonProperty("Progress Point")] public List<ProgressPoint> progressPoints = new ();
    }

    [SerializeField] private List<Object> _objects = new ();
    [SerializeField] private AIManager _AIManager;

    private int _progressIndex;

    public List<Object> Objects => _objects;

    #region MonoBehaviour
    private void Awake()
    {
        _progressIndex = PlayerPrefs.GetInt(Keys.Progress.ProgressIndex.ToString(), 0);

        FirstInitialize();
        JSON.ReadFileProgress(_objects);
        ActivateObjects();
    }
    #endregion

    public void IncreaseProgress()
    {
        if (_progressIndex == _objects.Count - 1)
        {
            return;
        }

        var progress = _objects[_progressIndex].progressPoints.Count(x => x.Cost > 0);
        if (progress == 0)
        {
            PlayerPrefs.SetInt(Keys.Progress.ProgressIndex.ToString(), ++_progressIndex);
        }
    }
    
    public void ShowNextObject()
    {
        if (_progressIndex >= _objects.Count)
        {
            return;
        }

        for (int i = 0; i < _objects[_progressIndex].progressPoints.Count; i++)
        {
            if (_objects[_progressIndex].progressPoints[i].Cost > 0)
            {
                _objects[_progressIndex].progressPoints[i].Show();
            }
        }
    }
    
    private void ActivateObjects()
    {
        for (int i = 0; i <= _progressIndex; i++)
        {
            for (int j = 0; j < _objects[i].progressPoints.Count; j++)
            {
                if (_objects[i].progressPoints[j].Cost <= 0)
                {
                    _objects[i].progressPoints[j].Activate();
                    if (_objects[i].progressPoints[j].TryGetComponent(out BuildingPoint buildingPoint))
                    {
                        _AIManager.AddRack(buildingPoint.Rack);
                    }
                }
                else
                {
                    _objects[i].progressPoints[j].Deactivate();
                }
            }
        }

        ShowNextObject();
    }

    private void FirstInitialize()
    {
        foreach (Object @object in _objects)
        {
            foreach (ProgressPoint point in @object.progressPoints)
            {
                point.Initialize(this);
            }
        }
    }
}

