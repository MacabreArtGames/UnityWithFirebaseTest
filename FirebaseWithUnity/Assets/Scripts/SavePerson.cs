using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using Firebase;
using Firebase.Storage;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using System;
using UnityEngine.UI.Extensions;


public class SavePerson : MonoBehaviour {
    FirebaseStorage firebaseStorage;

    [SerializeField] UserInfo userInfo;
    UserInfo _showeduserinfo;
    public InputField id_field;
    [Space]
    public InputField name_field;
    public InputField surname_field;
    public InputField age_field;
    public InputField education_field;
    public InputField nation_field;
    [Space]
    public InputField name_field2;
    public InputField surname_field2;
    public InputField age_field2;
    public InputField education_field2;
    public InputField nation_field2;

    DatabaseReference databaseReference;
    [Space]
    public GameObject ShowPanel;
    [Space]
    [Header("UI elements")]
    public Dropdown dropdown;
    public List<UserInfo> userInfos;
    public List<Text> userDisplayTexts;
    private bool IsFillLeaderList = false;

    DataSnapshot dataSnapshot;

    static InvokePump invoke = new InvokePump();
    [Space]
    public GameObject leaderBoardPanel;

    private void Start() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fir-testproject-6dc6e.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
    }

    public void SubmitUser() {
        /*
        name_field.text
        surname_field.text
        age_field.text
        education_field.text
        nation_field.text
            */
        userInfo = new UserInfo();

        userInfo.name = name_field.text;
        userInfo.surname = surname_field.text;
        userInfo.age = age_field.text;
        userInfo.education = education_field.text;
        userInfo.nation = nation_field.text;

        SaveUserToDB(userInfo);
    }

    void SaveUserToDB(UserInfo _userInfo) {
        string json = JsonUtility.ToJson(_userInfo);
        int i = UnityEngine.Random.Range(0, 1000000);
        string _i = i.ToString();

        databaseReference.Child("users").Child(_i).SetRawJsonValueAsync(json);
        ClearFields();
    }

    public void ShowUserFromDB() {
        string _id = id_field.text;
        if (_id.Length != 0) {
            FirebaseDatabase.DefaultInstance.GetReference("users").Child(_id).GetValueAsync().
                ContinueWith(task => {
                    if (task.IsFaulted) {
                        Debug.Log("Unsuccesful");

                    } else if (task.IsCompleted) {
                        DataSnapshot dataSnapshot = task.Result;
                        _showeduserinfo = new UserInfo();

                        string json = dataSnapshot.GetRawJsonValue();
                        _showeduserinfo = JsonUtility.FromJson<UserInfo>(json);
                        invoke.Invoke(() => ShowUserInfo(_showeduserinfo));

                    }
                });
        }
    }

    private void Update() {
        invoke.Update();
    }

    void ShowUserInfo(UserInfo __uinfo) {
        name_field2.text = __uinfo.name;
        surname_field2.text = __uinfo.surname;
        age_field2.text = __uinfo.age;
        education_field2.text = __uinfo.education;
        nation_field2.text = __uinfo.nation;
        _showeduserinfo = null;
    }

    void ClearFields() {
        name_field.text = "";
        surname_field.text = "";
        age_field.text = "";
        education_field.text = "";
        nation_field.text = "";
    }

    public void CloseShowPanel() {
        name_field2.text = "";
        surname_field2.text = "";
        age_field2.text = "";
        education_field2.text = "";
        nation_field2.text = "";
        ShowPanel.SetActive(false);

    }


    private void FillListWithUserNames(List<UserInfo> _userInfos) {
        for (int i = 0; i < _userInfos.Count; i++) {
            userDisplayTexts[i].text = String.Format("{0} {1} {2}", _userInfos[i].age, userInfos[i].name, userInfos[i].surname);
        }
        IsFillLeaderList = false;
    }


    public void OrderUsersByParam(Int32 _i) {
        string orderParam = "";
        switch (_i) {
            case 0:
                orderParam = "age";
                break;
            case 1:
                orderParam = "name";
                break;
            case 2:
                orderParam = "surname";
                break;
            case 3:
                orderParam = "education";
                break;
            case 4:
                orderParam = "nation";
                break;
        }


        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild(orderParam).LimitToFirst(10).GetValueAsync().
            ContinueWith(task => {
                if (task.IsFaulted) {
                    Debug.Log("Unsuccesful");
                } else if (task.IsCompleted) {
                    DataSnapshot dataSnapshot = task.Result;
                    invoke.Invoke(() => Fill(dataSnapshot));
                }
            });
    }

    private void Fill(DataSnapshot _dataSnapshot) {
        userInfos = new List<UserInfo>();
        foreach (DataSnapshot _d in _dataSnapshot.Children) {
            string _json = _d.GetRawJsonValue();
            _showeduserinfo = new UserInfo();
            _showeduserinfo = JsonUtility.FromJson<UserInfo>(_json);
            userInfos.Add(_showeduserinfo);
            FillListWithUserNames(userInfos);
        }
    }





}
