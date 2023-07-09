using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField]
    private float xLimit = 2.5f;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Animator anim;

    private int _direction = 0;

    public int Direction { get { return _direction; } }
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
    private KeywordRecognizer keywordRecognizer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        keywordActions.Add("stop", Stop);
        keywordActions.Add("left", Left);
        keywordActions.Add("right", Right);

        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
        keywordRecognizer.Start();
        
    }

    private void OnKeywordRecognized (PhraseRecognizedEventArgs args)
    {
        Debug.Log("Keyword: " + args.text);
        keywordActions[args.text].Invoke();
    }

    private void Stop()
    {
        _direction = 0;
    }

    private void Left()
    {
        _direction = -1;
    }

    private void Right()
    {
        _direction = 1;
    }
    // Update is called once per frame
    void Update()
    {
        // Keyboard input.
        float horizontalInput = Input.GetAxis("Horizontal");
        int directionKeyboard = (horizontalInput != 0) ? (int)Mathf.Sign(horizontalInput) : 0;

        // Combine keyboard and voice input. You might need to handle the case when both inputs are active.
        int combinedDirection = _direction != 0 ? _direction : directionKeyboard;

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
            MenuManager.instance.GameOver();
            gameObject.SetActive(false);
        }
    }
}