using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField]
    private float xLimit = 2.5f;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float dashDistance = 1f;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float dashCooldown = 5f;
    private float lastDashTime = -5f;

    private int currentDirection = 0;
    private int _direction = 0;
    private bool isInvincible = false;
    public TMP_Text dashCooldownText;
    public int Direction { get { return _direction; } }
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
    private KeywordRecognizer keywordRecognizer;

    private bool isDashing = false;
    private bool isMoving = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        keywordActions.Add("stop", Stop);
        keywordActions.Add("left", Left);
        keywordActions.Add("right", Right);
        keywordActions.Add("dash", Dash);

        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
        keywordRecognizer.Start();
    }

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Keyword: " + args.text);
        keywordActions[args.text].Invoke();
    }

    private void Stop()
    {
        _direction = 0;
        isMoving = false;
    }

    private void Left()
    {
        _direction = -1;
        isMoving = true;
    }

    private void Right()
    {
        _direction = 1;
        isMoving = true;
    }

    private void Dash()
    {
        if (Time.time >= lastDashTime + dashCooldown && isMoving && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        isInvincible = true;
        for (int i = 0; i < 3; i++)
        {
            float newPosX = transform.position.x + dashDistance * currentDirection;
            newPosX = Mathf.Clamp(newPosX, -xLimit, xLimit);
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
            yield return null;
        }
        isInvincible = false;
        isDashing = false;
        lastDashTime = Time.time;
    }

    void Update()
    {
        if (Time.time < lastDashTime + dashCooldown)
        {
            dashCooldownText.text = $"Cooldown: {Mathf.Max(0, lastDashTime + dashCooldown - Time.time):0.0}s";
        }
        else
        {
            dashCooldownText.text = "";
        }
        Vector3 textPosition = new Vector3(transform.position.x, transform.position.y + 2f, dashCooldownText.transform.position.z);
        dashCooldownText.transform.position = textPosition;

        float horizontalInput = Input.GetAxis("Horizontal");
        int directionKeyboard = (horizontalInput != 0) ? (int)Mathf.Sign(horizontalInput) : 0;

        int combinedDirection = _direction != 0 ? _direction : directionKeyboard;
        isMoving = combinedDirection != 0;

        if (combinedDirection != 0)
        {
            currentDirection = combinedDirection;
        }

        if (!isDashing && combinedDirection != 0)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime * combinedDirection;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xLimit, xLimit), transform.position.y, transform.position.z);
        }

        transform.localScale = new Vector3(combinedDirection != 0 ? combinedDirection : transform.localScale.x, 0.5f, 1);

        if (combinedDirection != 0)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime * combinedDirection;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xLimit, xLimit), transform.position.y, transform.position.z);
        }

        if (combinedDirection != 0)
        {
            anim.SetBool("Start", true);
        }
        else
        {
            anim.SetBool("Start", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            SoundManager.instance.PlayCoin();
            MenuManager.instance.IncreaseCoin();
            GameObject coinEffect = ObjectPooling.instance.GetPooledObject("CoinEffect");
            coinEffect.transform.position = transform.position;
            coinEffect.SetActive(true);

            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Enemy"))
        {
            if (!isInvincible)
            {
                MenuManager.instance.GameOver();
                gameObject.SetActive(false);
            }
        }
    }
}
