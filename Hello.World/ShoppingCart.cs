using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingCartServices.Models;
using ShoppingCartServices.Repository;

namespace ShoppingCartServices.Repository
{
    /// <summary>
    ///  Implemented IShoppingCart<DemoShoppingCart>
    /// </summary>
    public class ShoppingCart : IShoppingCart<DemoShoppingCart>
    {
        DemoShoppingCart Item = new DemoShoppingCart();
        public ServiceStatus ssStatus = new ServiceStatus();
        public DemoShoppingCart scShoppingCart = new DemoShoppingCart();
        private PayGovProdAzureEntities db = new PayGovProdAzureEntities();

        /// <summary>
        ///  ListShoppingCartItems
        ///  <purpose>
        ///   Passing in TTID retieve all Sessions accordingly 
        ///  </purpose>
        /// </summary>
        /// <param name="SessionId">@ALL@ or a sessionid</param>
        /// <returns>List<DemoShoppingCart></returns>
        public List<vw_DemoShoppingCarts> ListShoppingCartItems(string TTID, string Sort = "desc")
        {
            List<vw_DemoShoppingCarts> returnList = new List<vw_DemoShoppingCarts>();
            
            if (Sort.ToLower() == "desc")
            {
                returnList = (from c in db.vw_DemoShoppingCarts
                             where c.ttid == TTID
                           orderby c.ttid descending 
                            select c).ToList();
            }
            else
            {
                returnList = (from c in db.vw_DemoShoppingCarts
                             where c.ttid == TTID
                           orderby c.ttid ascending
                            select c).ToList();
            }

            return returnList;
        }

        /// <summary>
        ///  GetShoppingCartItems
        ///  <purpose>
        ///   Passing in @ALL@ or a session to retieve all rows accordingly 
        ///  </purpose>
        /// </summary>
        /// <param name="SessionId">@ALL@ or a sessionid</param>
        /// <returns>List<DemoShoppingCart></returns>
        public List<DemoShoppingCart> GetShoppingCartItems(string SessionId)
        {
            List<DemoShoppingCart> returnCart = new List<DemoShoppingCart>();
            var WholeCart = (from c in db.DemoShoppingCarts
                            where c.SessionID == SessionId 
                           select c).ToList();

            if (SessionId == "@ALL@")
            {
                WholeCart = (from c in db.DemoShoppingCarts
                           select c).ToList();
            }

            foreach (var cartItem in WholeCart)
            {
                returnCart.Add(cartItem);
            }
            return returnCart;
        }

        /// <summary>
        ///  GetShoppingCartItemsSummary
        ///  <purpose>
        ///   Colects totals for rows, amount, servicefees, and shippingfees 
        ///  </purpose>
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns>ServiceStatus</returns>
        public ServiceStatus GetShoppingCartItemsSummary(string SessionId)
        {
            ssStatus.RowsTotal = 0;
            ssStatus.ProductTotal = 0;
            ssStatus.RowsAffected = 0;
            ssStatus.Status = "Ignored";
            ssStatus.ServiceFeeTotal = 0;
            ssStatus.ShippingFeeTotal = 0;
            var WholeCart = (from c in db.DemoShoppingCarts
                             where c.SessionID == SessionId
                             select c).ToList();

            if (null != WholeCart)  
            {
                foreach (var cartItem in WholeCart)
                {
                    ssStatus.Status = "Success";
                    ssStatus.SessionID = SessionId;
                    ssStatus.RowsTotal += cartItem.Quantity; // Remeber to chance this to ItemsTotal ...
                    ssStatus.ProductTotal += cartItem.Price * cartItem.Quantity;
                    ssStatus.ServiceFeeTotal += cartItem.ServiceFee ?? 0;
                    ssStatus.ShippingFeeTotal += cartItem.ShippingFee ?? 0;
                    ssStatus.RowsAffected = 1;
                }
                return ssStatus;
            }

            return ssStatus;
        }
        /// <summary>
        ///  AddItemToShoppingCart
        ///  <purpose>
        ///   Add an item for a [Session] cart 
        ///   -OR- Increase Quantity by Product & Price.
        ///  </purpose>
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns>ServiceStatus</returns>
        public ServiceStatus AddItemToShoppingCart(DemoShoppingCart cCartItem)
        {
            int iRetCode = 0;
            ssStatus.RowsAffected = 1;
            ssStatus.Status = "Success";
            var CartItem = (from c in db.DemoShoppingCarts
                            where c.SessionID == cCartItem.SessionID
                               && c.ProductID == cCartItem.ProductID
                               && c.Price == cCartItem.Price
                           select c).FirstOrDefault();

            /*
             *  When we already have this product & price 
             *   Increase the quantity ... 
             */
            if (null != CartItem)  // Using Query          [CartItem]
            {
                CartItem.Quantity++;
            }
            /*
             *  Else we need to add a new cart Item ... 
             */
            else                  // Using Incomeing block [cCartItem]
            {
                cCartItem.Quantity = 1;
                db.DemoShoppingCarts.Add(cCartItem);
            }

            try
            {
                iRetCode = db.SaveChanges();
                ssStatus = GetShoppingCartItemsSummary(cCartItem.SessionID);
            }
            catch (Exception ex)
            {
                ssStatus.RetCode = iRetCode;
                ssStatus.Status = "Failure";
                ssStatus.Reason = ex.InnerException.ToString();
            }

            return ssStatus;
        }

        /// <summary>
        ///  RemoveItemFromShoppingCart
        ///  <purpose>
        ///   Decrease an items (Product & Price) quantity 
        ///   -OR- Remove an item for a [Session] cart 
        ///  </purpose>
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns>ServiceStatus</returns>
        public ServiceStatus RemoveItemFromShoppingCart(DemoShoppingCart cCartItem)
        {
            int iRetCode = 0;
            ssStatus.RowsAffected = 1;
            ssStatus.Status = "Ignored";
            var CartItem = (from c in db.DemoShoppingCarts
                            where c.SessionID == cCartItem.SessionID
                               && c.ProductID == cCartItem.ProductID
                               && c.Price == cCartItem.Price
                           select c).FirstOrDefault();

            /*
             *  When we already have this product & price 
             *   Remove the cart Item or Deccrease its quantity ...
             */
            if (null != CartItem)  // Using Query          [CartItem]
            {
                ssStatus.RowsAffected = 1;
                ssStatus.Status = "Success";
                if (CartItem.Quantity == 1)
                {
                    db.DemoShoppingCarts.Remove(cCartItem);
                }
                /*
                 *  Else we need to just Decrease the quantity ... 
                 */
                else              // Using Incomeing block [cCartItem]
                {
                    CartItem.Quantity--;
                }

                try
                {
                    iRetCode = db.SaveChanges();
                    ssStatus = GetShoppingCartItemsSummary(cCartItem.SessionID);
                }
                catch (Exception ex)
                {
                    ssStatus.RetCode = iRetCode;
                    ssStatus.Status = "Failure";
                    ssStatus.Reason = ex.InnerException.ToString();
                }
            }

            return ssStatus;
        }

        /// <summary>
        ///  PutShoppingCart
        ///  <purpose>
        ///   RESERVED for cloneing a new cart or maintenance
        ///  </purpose>
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public DemoShoppingCart GetCartItem(int id)
        {
            DemoShoppingCart CartItem = (from c in db.DemoShoppingCarts
                                        where c.DemoShoppingCartID == id
                                       select c).FirstOrDefault();
            return CartItem;
        }

        /// <summary>
        ///  PutShoppingCart
        ///  <purpose>
        ///   RESERVED for cloneing a new cart or maintenance
        ///  </purpose>
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public ServiceStatus AddItemToShoppingCart(List<DemoShoppingCart> cart)
        {
            int rowCount = 1;
            List<DemoShoppingCart> returnCart = new List<DemoShoppingCart>();
            ssStatus.RetCode = 0;
            ssStatus.Status = "Success";
            ssStatus.Reason = "cart was put away Successfuly";
            foreach (var cartItem in cart)
            {
                db.DemoShoppingCarts.Add(cartItem);
                ssStatus.RowsAffected = rowCount++;
            }
            try
            {
                int num = db.SaveChanges();
            }
            catch (Exception ex)
            {
                ssStatus.RetCode = 99;
                ssStatus.Status = "Failure";
                ssStatus.Reason = ex.InnerException.ToString();
                ssStatus.RowsAffected = rowCount - 1;
            }
            return ssStatus;
        }

        /// <summary>
        ///  DeleteShoppingCart
        ///  <purpose>
        ///   RESERVED for cloneing a new cart or maintenance
        ///  </purpose>
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public ServiceStatus DeleteShoppingCart(string SessionId)
        {
            int iRetCode = 0;
            var WholeCart = (from c in db.DemoShoppingCarts
                             where c.SessionID == SessionId
                             select c).ToList();

            if (null != WholeCart)
            {
                foreach (var cartItem in WholeCart)
                {
                    try
                    {
                        db.DemoShoppingCarts.Remove(cartItem);
                        iRetCode = db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ssStatus.RetCode = iRetCode;
                        ssStatus.Status = "Failure";
                        ssStatus.Reason = ex.InnerException.ToString();
                        ssStatus.RowsAffected = 1;
                    }
                }
            }

            return ssStatus;
        }
    }
}
