using UnityEngine;

/* [0] 개요 : IDamageable
		- 데미지와 관련된 모든 내용을 공유함.
*/

public interface IDamageable
{
    public void TakeDamage(float damage);
    public void Slow(float rate);
}
