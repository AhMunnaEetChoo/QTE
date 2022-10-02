using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ScrollingMaterialDir : MonoBehaviour
{
	public float XSpeed;
	public float YSpeed;

	float _uvFactor;
	Material _materialToScroll;

	void Awake()
	{
		_materialToScroll = GetComponent<MeshRenderer>().material;
		float n = _materialToScroll.GetTextureScale("_MainTex").y;
		float scale = transform.localScale.z;
		_uvFactor = n / scale;
	}

	void Update()
	{
		_materialToScroll.mainTextureOffset = new Vector2(_materialToScroll.mainTextureOffset.x + Time.deltaTime * XSpeed * _uvFactor, _materialToScroll.mainTextureOffset.y + Time.deltaTime * YSpeed * _uvFactor);
	}
}