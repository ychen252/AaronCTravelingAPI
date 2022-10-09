using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Stateless;

namespace AaCTraveling.API.Models
{
    public class Order
    {
        public Order()
        {
            StateMachineInit();    
        }
        
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }
        
        public void PaymentProcessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PlaceOrder);
        }

        public void PaymentApproved()
        {
            _machine.Fire(OrderStateTriggerEnum.Approve);
        }

        public void PaymentReject()
        {
            _machine.Fire(OrderStateTriggerEnum.Reject);
        }

        StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine; 
        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(
                () => State,
                s => State = s);

            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refunding);
        }
    }
    
    public enum OrderStateEnum
    {
        Pending,
        Processing,
        Completed,
        Declined,
        Cancelled,
        Refunding
    }

    public enum OrderStateTriggerEnum
    {
        PlaceOrder, // pending -> processing
        Approve, // processing -> completed
        Reject, // processing -> Declined
        Cancel, //processing/pending -> cancelled
        Return // completed -> refund
    }
}
