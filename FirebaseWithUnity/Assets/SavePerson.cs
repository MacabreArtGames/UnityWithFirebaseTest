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
        int i = Random.Range(0, 1000000);
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
                        //_userinfo.name = dataSnapshot.Child("name").Value.ToString();

                        string json = dataSnapshot.GetRawJsonValue();
                        _showeduserinfo = JsonUtility.FromJson<UserInfo>(json);


                    }
                });
        }

    }

    private void Update() {
        if (_showeduserinfo != null) {
            ShowUserInfo(_showeduserinfo);
        }
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

    public  void CloseShowPanel() {
        name_field2.text = "";
        surname_field2.text = "";
        age_field2.text = "";
        education_field2.text = "";
        nation_field2.text = "";
        ShowPanel.SetActive(false);

    }

}
