using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �ӽ� ���� ����Ÿ ���̽���,
 * ���߿� ���� �������� ��Ʈ�� �鿣�� ������ �� �������ش�.
 * ���̵� ���� �������� ��Ʈ���� ����Ÿ �ҷ��ͼ� SO�� �������ش�.
 * �׸��� �ش� SO ������ ����ؼ� �⹰ ����Ÿ�� ����Ѵ�.
 */


[CreateAssetMenu(fileName = "UserDataSO", menuName = "Scriptable Object/UserDataSO")]
public class UserDataSO : ScriptableObject
{
    public List<string> curMarblesId;
}
