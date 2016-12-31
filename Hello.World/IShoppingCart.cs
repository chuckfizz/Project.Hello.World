using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingCartServices.Models;

namespace ShoppingCartServices.Repository
{
    /// <summary>
    /// ShoppingCart class
    /// <purpose>
    ///    Three state; 
    ///         1) Empty
    ///             i.    Can Add an item
    ///         2) Active
    ///             i.    Can Add an item
    ///             ii.   Can Remove an item
    ///             iii.  Can Increase an item by equal Product/Price
    ///             iv.   Can Decrease an item by equal Product/Price
    ///         3) Paid (Closed)
    /// </purpose>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IShoppingCart<T>
    {
        // Get all Items in a cart by SessionId
        List<T> GetShoppingCartItems(string SessionId);

        // Get all Items in a cart by SessionId
        ServiceStatus GetShoppingCartItemsSummary(string SessionId);

        // Add an item for a [Session] cart 
        // -OR- Increase Quantity by Product & Price.
        ServiceStatus AddItemToShoppingCart(T cCartItem);

        // Decrease an items (Product & Price) quantity 
        // -OR- Remove an item for a [Session] cart 
        ServiceStatus RemoveItemFromShoppingCart(T cCartItem);
    }
}
