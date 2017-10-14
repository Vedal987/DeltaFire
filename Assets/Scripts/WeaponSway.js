#pragma strict

var amount : float = 0.02;
var maxAmount : float = 0.03;
var smooth : float = 3;
private var def : Vector3;
 
function Start (){
    def = transform.localPosition;
}
 
function Update (){
 
        var factorX : float = -Input.GetAxis("Mouse X") * amount;
        var factorY : float = -Input.GetAxis("Mouse Y") * amount;
       
        factorX = Mathf.Clamp(factorX, -maxAmount, maxAmount);
        factorY = Mathf.Clamp(factorY, -maxAmount, maxAmount);
 
        var Final : Vector3 = new Vector3(def.x+factorX, def.y+factorY, def.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * smooth);       
}