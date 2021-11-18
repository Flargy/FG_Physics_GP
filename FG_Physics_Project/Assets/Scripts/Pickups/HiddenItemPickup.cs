public class HiddenItemPickup : PickupBase
{
   protected override void Activate()
   {
      if(UI_Manager.Instance)
         UI_Manager.Instance.AddHiddenItem();
      Destroy(gameObject);
   }
}
