using UnityEngine;

public class IncreaseRound : MonoBehaviour
{
    [SerializeField] radialFillManager _magicFillManager;
    [SerializeField] radialFillManager _stamFillManager;
    [SerializeField] int _maxRound = 20;

    [Header("Sprint Settings")]
    [SerializeField] float _stamDrainRate = 2f;
    [SerializeField] float _stamRegenRate = 1f;
    [SerializeField] float _sprintCooldown = 2f;
    [SerializeField] KeyCode _sprintKey = KeyCode.LeftShift;

    [Header("Magic Settings")]
    [SerializeField] float _magicRegenRate = 0.5f;

    private float _currentMagic;   // changed to float for smooth regen
    private float _currentStam;
    private float _sprintCooldownTimer = 0f;

    public bool CanSprint => _currentStam > 0 && _sprintCooldownTimer <= 0f;
    public bool CanShoot => _currentMagic > 0;
    public bool IsSprinting { get; private set; }

    void Start()
    {
        _currentMagic = _maxRound;
        _currentStam = _maxRound;
    }

    void Update()
    {
        HandleSprint();
        HandleMagicRegen();
    }

    public void UseMagic()
    {
        _currentMagic = Mathf.Max(_currentMagic - 3, 0f);
        _magicFillManager.UpdateRadialProgressCircle(Mathf.RoundToInt(_currentMagic), _maxRound);
    }

    private void HandleSprint()
    {
        if (_sprintCooldownTimer > 0f)
            _sprintCooldownTimer -= Time.deltaTime;

        bool wantsToSprint = Input.GetKey(_sprintKey);

        if (wantsToSprint && CanSprint)
        {
            IsSprinting = true;
            _currentStam -= _stamDrainRate * Time.deltaTime;
            _currentStam = Mathf.Max(_currentStam, 0f);

            if (_currentStam <= 0f)
                _sprintCooldownTimer = _sprintCooldown;
        }
        else
        {
            IsSprinting = false;
            _currentStam += _stamRegenRate * Time.deltaTime;
            _currentStam = Mathf.Min(_currentStam, _maxRound);
        }

        _stamFillManager.UpdateRadialProgressCircle(Mathf.RoundToInt(_currentStam), _maxRound);
    }

    private void HandleMagicRegen()
    {
        if (_currentMagic < _maxRound)
        {
            _currentMagic += _magicRegenRate * Time.deltaTime;  // float accumulation, no rounding loss
            _currentMagic = Mathf.Min(_currentMagic, _maxRound);
            _magicFillManager.UpdateRadialProgressCircle(Mathf.RoundToInt(_currentMagic), _maxRound);
        }
    }
}