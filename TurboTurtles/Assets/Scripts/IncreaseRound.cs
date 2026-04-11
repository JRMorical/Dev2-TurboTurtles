using UnityEngine;

public class IncreaseRound : MonoBehaviour
{
    [SerializeField] radialFillManager _magicFillManager;
    [SerializeField] radialFillManager _stamFillManager;
    [SerializeField] int _maxRound = 20;

    private int _currentRound = 0;

    public void IncreaseFill()
    {
        _currentRound = Mathf.Min(_currentRound + 1, _maxRound);
        UpdateBoth();
    }

    public void DecreaseFill()
    {
        _currentRound = Mathf.Max(_currentRound - 1, 0);
        UpdateBoth();
    }

    private void UpdateBoth()
    {
        _magicFillManager.UpdateRadialProgressCircle(_currentRound, _maxRound);
        _stamFillManager.UpdateRadialProgressCircle(_currentRound, _maxRound);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) IncreaseFill();
        if (Input.GetKeyDown(KeyCode.Q)) DecreaseFill();
    }
}