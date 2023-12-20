using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 임시 유저 데이타 베이스로,
 * 나중에 구글 스프레드 시트로 백엔드 구축할 때 변경해준다.
 * 아이디에 따라 스프레드 시트에서 데이타 불러와서 SO에 저장해준다.
 * 그리고 해당 SO 정보를 사용해서 기물 데이타를 사용한다.
 */


[CreateAssetMenu(fileName = "UserDataSO", menuName = "Scriptable Object/UserDataSO")]
public class UserDataSO : ScriptableObject
{
    public List<string> curMarblesId;
}
