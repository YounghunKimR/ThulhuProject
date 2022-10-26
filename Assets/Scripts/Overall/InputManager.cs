using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{    
    public static InputManager inst;
    private static Queue<InputUnit> inputQueue = new Queue<InputUnit>();
    private static Dictionary<string, InputUnit> inputDict = new Dictionary<string, InputUnit>();
    private static int inputLastingFrame = 10;

    //싱글턴
    void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(gameObject);
        }
    }

    void Start(){}

    // Update is called once per frame
    void Update()
    {
        InputLogic();
    }


    //사용자가 스페이스를 누르지 않았음에도 연출적으로 예상하지 못한 타이밍에 대화를 진행시키기 위한 메소드
    public void PressSpace()
    {
        inputDict.Add("space", new InputUnit(KeyCode.Space, StartCoroutine(RemoveAfterLastingFrame("space"))));
    }
    //inputName으로 dict에 등록된 inputUnit이 있는지 검색하고 있다면 true를 반환
    public bool GetInput(string inputName)
    {
        bool isExist = inputDict.ContainsKey(inputName);
        if(!isExist)
            return false;
        InputUnit iUnit = inputDict[inputName];
        
        if(iUnit.isUsed == false){
            //lastingframe 수 만큼 지나도 사용되지 않으면 dict에서 제거하는 코루틴을 정지시키고 
            //endofframe에서 사용한 inputunit을 제거하는 코루틴을 실행한다.
            StopCoroutine(iUnit.removeCor);
            iUnit.isUsed = true;
            StartCoroutine(RemoveImmediately("space"));
        }
        return true;
    }

    //매 프레임마다 입력된 키코드 정보를 inputQueue에 enqueue한다. 10프레임 이내로 사용되지 않는다면 dequeue시킨다.
    private void InputLogic()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PressSpace();
        }
    }

    private IEnumerator RemoveImmediately(string inputName)
    {
        yield return new WaitForEndOfFrame();
        inputDict.Remove(inputName);
    }
    private IEnumerator RemoveAfterLastingFrame(string inputName)
    {
        for (int i = 0; i < inputLastingFrame; i++)
        {
            yield return null;
        }
        inputDict.Remove(inputName);
    }
}



public class InputUnit
{
    public InputType inputType = InputType.Keyboard;
    public KeyCode keyCode;
    public int mouseBtn = 0;
    public Vector3 mousePos;
    public Coroutine removeCor;
    public bool isUsed = false;

    public InputUnit(KeyCode keyCode, Coroutine removeCor){
        this.inputType = InputType.Keyboard;
        this.keyCode = keyCode;
        this.removeCor = removeCor;
    }

    public InputUnit(int mouseBtn, Vector3 mousePos, Coroutine removeCor){
        this.inputType = InputType.Mouse;
        this.mouseBtn = mouseBtn;
        this.mousePos = mousePos;
        this.removeCor = removeCor;
    }
    

    // override object.Equals
    public override bool Equals(object obj)
    {
        bool isEqual = false;
        if (obj == null || GetType() != obj.GetType())
        {
            return isEqual;
        }
        InputUnit unit = obj as InputUnit;
        if (inputType == unit.inputType)
        {
            switch (inputType){
                case InputType.Keyboard:
                    if(keyCode == unit.keyCode)
                        isEqual = true;
                    break;
                case InputType.Mouse:
                    if(mouseBtn == unit.mouseBtn)
                        isEqual = true;
                    break;
                case InputType.Touch:
                        isEqual = true;
                    break;
                default:
                    throw new System.Exception("Input Error has occured. \nerror code : 1");
            }
            return true;
        }
        return false;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}



public enum InputType{
    Keyboard, 
    Mouse, 
    Touch
}