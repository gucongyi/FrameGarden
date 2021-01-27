using Google.Protobuf;
using System.Collections.Generic;

namespace Game.Protocal
{
    //三者1:1:1关系
    public class OPRelation
    {
        public OpCodeType codeType;
        public System.Type typeRequest;
        public System.Type typeResponse;
    }
    public class ListOPRelation
    {
        public static List<OPRelation> listOpRelation = new List<OPRelation>()
        {
            new OPRelation(){codeType=(OpCodeType)1,typeRequest=typeof(CSLinkInfo),typeResponse=typeof(SCLinkInfo)},
			new OPRelation(){codeType=(OpCodeType)2,typeRequest=typeof(CSEmptyAccountInfo),typeResponse=typeof(SCUserInfo)},
			new OPRelation(){codeType=(OpCodeType)3,typeRequest=typeof(CSSetBasicsInfo),typeResponse=typeof(SCEmtpySetBasicsInfo)},
			new OPRelation(){codeType=(OpCodeType)5,typeRequest=typeof(CSEmptySCFriendList),typeResponse=typeof(SCFriendList)},
			new OPRelation(){codeType=(OpCodeType)6,typeRequest=typeof(CSEmptyApplyList),typeResponse=typeof(SCApplyList)},
			new OPRelation(){codeType=(OpCodeType)7,typeRequest=typeof(CSEmptyRecommendList),typeResponse=typeof(SCRecommendList)},
			new OPRelation(){codeType=(OpCodeType)8,typeRequest=typeof(CSApply),typeResponse=typeof(SCEmptyApply)},
			new OPRelation(){codeType=(OpCodeType)9,typeRequest=typeof(CSAccept),typeResponse=typeof(SCEmptyAccept)},
			new OPRelation(){codeType=(OpCodeType)10,typeRequest=typeof(CSRepulse),typeResponse=typeof(SCEmptyRepluse)},
			new OPRelation(){codeType=(OpCodeType)11,typeRequest=typeof(CSDelFriend),typeResponse=typeof(SCEmtpyDelFriend)},
			new OPRelation(){codeType=(OpCodeType)12,typeRequest=typeof(CSSearch),typeResponse=typeof(SCSearch)},
			new OPRelation(){codeType=(OpCodeType)13,typeRequest=typeof(CSEnterMap),typeResponse=typeof(SCSearch)},
			new OPRelation(){codeType=(OpCodeType)14,typeRequest=typeof(CSDice),typeResponse=typeof(SCDiceResult)},
			new OPRelation(){codeType=(OpCodeType)15,typeRequest=typeof(CSEmptyRichManFinish),typeResponse=typeof(SCEmptyRichManFinish)},
			new OPRelation(){codeType=(OpCodeType)16,typeRequest=typeof(CSLucky),typeResponse=typeof(SCLucky)},
			new OPRelation(){codeType=(OpCodeType)17,typeRequest=typeof(CSEmptyCropSpeed),typeResponse=typeof(SCEmptyCropSpeed)},
			new OPRelation(){codeType=(OpCodeType)18,typeRequest=typeof(CSBuyProp),typeResponse=typeof(SCBuyProp)},
			new OPRelation(){codeType=(OpCodeType)19,typeRequest=typeof(CSDrag),typeResponse=typeof(SCDrag)},
			new OPRelation(){codeType=(OpCodeType)20,typeRequest=typeof(CSEmptyManorInfo),typeResponse=typeof(SCManorData)},
			new OPRelation(){codeType=(OpCodeType)21,typeRequest=typeof(CSFertilizer),typeResponse=typeof(SCEmptFertilizer)},
			new OPRelation(){codeType=(OpCodeType)22,typeRequest=typeof(CSEradicate),typeResponse=typeof(SCEmptEradicate)},
			new OPRelation(){codeType=(OpCodeType)23,typeRequest=typeof(CSPlantData),typeResponse=typeof(SCPlantResult)},
			new OPRelation(){codeType=(OpCodeType)24,typeRequest=typeof(CSHarvestData),typeResponse=typeof(SCHarvestData)},
			new OPRelation(){codeType=(OpCodeType)25,typeRequest=typeof(CSStealData),typeResponse=typeof(SCStealData)},
			new OPRelation(){codeType=(OpCodeType)26,typeRequest=typeof(CSChangeLocation),typeResponse=typeof(SCEmptChangeLocationData)},
			new OPRelation(){codeType=(OpCodeType)27,typeRequest=typeof(CSGoodSellData),typeResponse=typeof(SCGoodSellData)},
			new OPRelation(){codeType=(OpCodeType)28,typeRequest=typeof(CSGoodLock),typeResponse=typeof(SCEmptGoodLock)},
			new OPRelation(){codeType=(OpCodeType)29,typeRequest=typeof(CSQueryOther),typeResponse=typeof(SCManorFriendData)},
			new OPRelation(){codeType=(OpCodeType)30,typeRequest=typeof(CSUnlockArea),typeResponse=typeof(SCUnlockArea)},
			new OPRelation(){codeType=(OpCodeType)31,typeRequest=typeof(CSEmptyRoleList),typeResponse=typeof(CSRoleSInfo)},
			new OPRelation(){codeType=(OpCodeType)32,typeRequest=typeof(CSChoiceRole),typeResponse=typeof(SCEmptChoiceRole)},
			new OPRelation(){codeType=(OpCodeType)33,typeRequest=typeof(CSRoleBuy),typeResponse=typeof(SCBuyProp)},
			new OPRelation(){codeType=(OpCodeType)34,typeRequest=typeof(CSEmptyPromotionsGoodsInfo),typeResponse=typeof(SCPromotionsGoodsInfo)},
			new OPRelation(){codeType=(OpCodeType)35,typeRequest=typeof(CSGoldJewelBuy),typeResponse=typeof(SCGoldJewelBuy)},
			new OPRelation(){codeType=(OpCodeType)36,typeRequest=typeof(CSBuySection),typeResponse=typeof(SCBuySection)},
			new OPRelation(){codeType=(OpCodeType)37,typeRequest=typeof(CSClearance),typeResponse=typeof(SCEmptyClearance)},
			new OPRelation(){codeType=(OpCodeType)38,typeRequest=typeof(CSOrnamentalRecycle),typeResponse=typeof(SCEmptyOrnamentalRecycle)},
			new OPRelation(){codeType=(OpCodeType)39,typeRequest=typeof(CSTreasureChest),typeResponse=typeof(SCTreasureChestResult)},
			new OPRelation(){codeType=(OpCodeType)40,typeRequest=typeof(CSGetTreasureChestAward),typeResponse=typeof(SCGetTreasureChestAwardResult)},
			new OPRelation(){codeType=(OpCodeType)43,typeRequest=typeof(CSBuyExtraStory),typeResponse=typeof(SCBuyExtraStory)},
			new OPRelation(){codeType=(OpCodeType)44,typeRequest=typeof(CSEmptyMail),typeResponse=typeof(SCMailInfo)},
			new OPRelation(){codeType=(OpCodeType)45,typeRequest=typeof(CSAccessoryInWarehouse),typeResponse=typeof(SCAccessoryInWarehouse)},
			new OPRelation(){codeType=(OpCodeType)46,typeRequest=typeof(CSAccessoryInfo),typeResponse=typeof(SCAccessory)},
			new OPRelation(){codeType=(OpCodeType)47,typeRequest=typeof(CSEmptySignIn),typeResponse=typeof(SCEverydayAward)},
			new OPRelation(){codeType=(OpCodeType)49,typeRequest=typeof(CSEmptyFriendSteal),typeResponse=typeof(SCFriendStealInfo)},
			new OPRelation(){codeType=(OpCodeType)50,typeRequest=typeof(CSEmptyUserUpGrade),typeResponse=typeof(SCCurrentExperience)},
			new OPRelation(){codeType=(OpCodeType)53,typeRequest=typeof(CSWorkShedSpeedUp),typeResponse=typeof(SCWorkShedSpeedUp)},
			new OPRelation(){codeType=(OpCodeType)54,typeRequest=typeof(CSEmptyWarehouseUnlockInfo),typeResponse=typeof(SCTreasureChestUnlockInfo)},
			new OPRelation(){codeType=(OpCodeType)55,typeRequest=typeof(CSTreasureChestSpeed),typeResponse=typeof(SCTreasureChestSpeed)},
			new OPRelation(){codeType=(OpCodeType)56,typeRequest=typeof(CSUseWarehouseGoods),typeResponse=typeof(SCUseWarehouseGoods)},
			new OPRelation(){codeType=(OpCodeType)57,typeRequest=typeof(CSEmptyManorLogs),typeResponse=typeof(SCManorLogs)},
			new OPRelation(){codeType=(OpCodeType)58,typeRequest=typeof(CSPurchaseRole),typeResponse=typeof(SCPurchaseRole)},
			new OPRelation(){codeType=(OpCodeType)59,typeRequest=typeof(CSSwitchoverRole),typeResponse=typeof(SCEmptySwitchoverRole)},
			new OPRelation(){codeType=(OpCodeType)60,typeRequest=typeof(CSSwitchoverCostume),typeResponse=typeof(SCEmptySwitchoverCostume)},
			new OPRelation(){codeType=(OpCodeType)61,typeRequest=typeof(CSOneselfMarmot),typeResponse=typeof(SCOneselfMarmot)},
			new OPRelation(){codeType=(OpCodeType)62,typeRequest=typeof(CSFriendMarmot),typeResponse=typeof(SCFriendMarmot)},
			new OPRelation(){codeType=(OpCodeType)63,typeRequest=typeof(CSAdvExtraStory),typeResponse=typeof(SCEmtpyAdvExtraStory)},
			new OPRelation(){codeType=(OpCodeType)64,typeRequest=typeof(CSWorldChat),typeResponse=typeof(SCEmptyWorldChat)},
			new OPRelation(){codeType=(OpCodeType)65,typeRequest=typeof(CSPrivateChat),typeResponse=typeof(SCEmptyPrivateChat)},
			new OPRelation(){codeType=(OpCodeType)66,typeRequest=typeof(CSEntranceRoom),typeResponse=typeof(SCEntranceRoom)},
			new OPRelation(){codeType=(OpCodeType)67,typeRequest=typeof(CSGuessing),typeResponse=typeof(SCEmtpyGuessing)},
			new OPRelation(){codeType=(OpCodeType)68,typeRequest=typeof(CSChannelChat),typeResponse=typeof(SCEmtpyChannelChat)},
			new OPRelation(){codeType=(OpCodeType)69,typeRequest=typeof(CSEmptyRoomList),typeResponse=typeof(SCRoomListInfo)},
			new OPRelation(){codeType=(OpCodeType)70,typeRequest=typeof(CSGetLoginFavorable),typeResponse=typeof(SCEmptyLoginGetFavorable)},
			new OPRelation(){codeType=(OpCodeType)71,typeRequest=typeof(CSSendNPCGift),typeResponse=typeof(SCEmptySendNPCGift)},
			new OPRelation(){codeType=(OpCodeType)72,typeRequest=typeof(CSSectionAddFavorable),typeResponse=typeof(SCEmtpySectionAddFavorable)},
			new OPRelation(){codeType=(OpCodeType)74,typeRequest=typeof(CSEmptyFavorableInfo),typeResponse=typeof(SCFavorableInfo)},
			new OPRelation(){codeType=(OpCodeType)75,typeRequest=typeof(CSMoveLocation),typeResponse=typeof(SCEmtpyMoveLocation)},
			new OPRelation(){codeType=(OpCodeType)76,typeRequest=typeof(CSEmptyDepartureRoom),typeResponse=typeof(SCEmptyDepartureRoom)},
			new OPRelation(){codeType=(OpCodeType)78,typeRequest=typeof(CSMotion),typeResponse=typeof(SCEmptyMotion)},
			new OPRelation(){codeType=(OpCodeType)79,typeRequest=typeof(CSEmptyMarmotAwardInfo),typeResponse=typeof(SCMarmotAwardInfo)},
			new OPRelation(){codeType=(OpCodeType)80,typeRequest=typeof(CSChangeMailState),typeResponse=typeof(SCEmtpyChangeMailState)},
			new OPRelation(){codeType=(OpCodeType)81,typeRequest=typeof(CSBuyPropInfo),typeResponse=typeof(SCBuyProp)},
			new OPRelation(){codeType=(OpCodeType)82,typeRequest=typeof(CSEmptyHeartBeat),typeResponse=typeof(SCEmtpyHeartBeat)},
			new OPRelation(){codeType=(OpCodeType)83,typeRequest=typeof(CSAccumulateSignIn),typeResponse=typeof(SCEmtpyAccumulateSignIn)},
			new OPRelation(){codeType=(OpCodeType)84,typeRequest=typeof(CSSaveGuidance),typeResponse=typeof(SCEmptySaveGuidance)},
			new OPRelation(){codeType=(OpCodeType)87,typeRequest=typeof(CSEmptyManorGuidance),typeResponse=typeof(SCManorGuidance)},
			new OPRelation(){codeType=(OpCodeType)88,typeRequest=typeof(CSEmptyStealManorGuidance),typeResponse=typeof(SCStealManorGuidance)},
			new OPRelation(){codeType=(OpCodeType)91,typeRequest=typeof(CSEmptyGetTaskInfo),typeResponse=typeof(SCGetTaskInfo)},
			new OPRelation(){codeType=(OpCodeType)92,typeRequest=typeof(CSGetTaskAward),typeResponse=typeof(SCGetTaskAward)},
			new OPRelation(){codeType=(OpCodeType)93,typeRequest=typeof(CSEmptyLogoutAccount),typeResponse=typeof(SCEmptyLogoutAccount)},
			new OPRelation(){codeType=(OpCodeType)94,typeRequest=typeof(CSCdkAward),typeResponse=typeof(SCCdkAward)},
			new OPRelation(){codeType=(OpCodeType)95,typeRequest=typeof(CSEmptyDealInfo),typeResponse=typeof(SCDealInfo)},
			new OPRelation(){codeType=(OpCodeType)96,typeRequest=typeof(CSSubmintDeal),typeResponse=typeof(SCSubmintDeal)},
			new OPRelation(){codeType=(OpCodeType)97,typeRequest=typeof(CSRefreshDeal),typeResponse=typeof(SCRefreshDeal)},
			new OPRelation(){codeType=(OpCodeType)98,typeRequest=typeof(CSAdvSkipDeal),typeResponse=typeof(SCEmptyAdvSkipDeal)},
			new OPRelation(){codeType=(OpCodeType)99,typeRequest=typeof(CSEntranceSection),typeResponse=typeof(SCEmptyEntranceSection)},
			new OPRelation(){codeType=(OpCodeType)100,typeRequest=typeof(CSEmptyCropMature),typeResponse=typeof(SCCropMature)},
			new OPRelation(){codeType=(OpCodeType)101,typeRequest=typeof(CSManorDecorateRotate),typeResponse=typeof(SCEmptyManorDecorateRotate)},
			new OPRelation(){codeType=(OpCodeType)102,typeRequest=typeof(CSLike),typeResponse=typeof(SCEmptyLike)},
			new OPRelation(){codeType=(OpCodeType)103,typeRequest=typeof(CSFriendCostume),typeResponse=typeof(SCFriendCostume)},
			new OPRelation(){codeType=(OpCodeType)104,typeRequest=typeof(CSEmptyOnceWatering),typeResponse=typeof(SCOnceWatering)},
			new OPRelation(){codeType=(OpCodeType)105,typeRequest=typeof(CSDerectUp),typeResponse=typeof(SCEmptyDerectUp)},
			new OPRelation(){codeType=(OpCodeType)50002,typeRequest=typeof(IMessage),typeResponse=typeof(SCFriendApplyPushMsg)},
			new OPRelation(){codeType=(OpCodeType)50003,typeRequest=typeof(IMessage),typeResponse=typeof(SCFriendAcceptPushMsg)},
			new OPRelation(){codeType=(OpCodeType)50004,typeRequest=typeof(IMessage),typeResponse=typeof(SCFriendRepulsePushMsg)},
			new OPRelation(){codeType=(OpCodeType)50005,typeRequest=typeof(IMessage),typeResponse=typeof(SCFriendDeletePushMsg)},
			new OPRelation(){codeType=(OpCodeType)50006,typeRequest=typeof(IMessage),typeResponse=typeof(SCSendMailPushMsg)},
			new OPRelation(){codeType=(OpCodeType)50009,typeRequest=typeof(IMessage),typeResponse=typeof(SCNotePushMess)},
			new OPRelation(){codeType=(OpCodeType)50010,typeRequest=typeof(IMessage),typeResponse=typeof(SCNotePushMess)},
			new OPRelation(){codeType=(OpCodeType)50011,typeRequest=typeof(IMessage),typeResponse=typeof(SCChat)},
			new OPRelation(){codeType=(OpCodeType)50012,typeRequest=typeof(IMessage),typeResponse=typeof(SCChat)},
			new OPRelation(){codeType=(OpCodeType)50013,typeRequest=typeof(IMessage),typeResponse=typeof(SCChat)},
			new OPRelation(){codeType=(OpCodeType)50014,typeRequest=typeof(IMessage),typeResponse=typeof(SCEntranceRoomInfo)},
			new OPRelation(){codeType=(OpCodeType)50015,typeRequest=typeof(IMessage),typeResponse=typeof(SCMoveLocation)},
			new OPRelation(){codeType=(OpCodeType)50016,typeRequest=typeof(IMessage),typeResponse=typeof(SCpushGuessingInfo)},
			new OPRelation(){codeType=(OpCodeType)50017,typeRequest=typeof(IMessage),typeResponse=typeof(SCpushMotion)},
			new OPRelation(){codeType=(OpCodeType)50018,typeRequest=typeof(IMessage),typeResponse=typeof(SCDepartureRoom)},
			new OPRelation(){codeType=(OpCodeType)50019,typeRequest=typeof(IMessage),typeResponse=typeof(SCActivityFinish)},
			new OPRelation(){codeType=(OpCodeType)50020,typeRequest=typeof(IMessage),typeResponse=typeof(SCRepetitionRegister)},
			new OPRelation(){codeType=(OpCodeType)50021,typeRequest=typeof(IMessage),typeResponse=typeof(SCEmtpyMailPushMsg)},
			new OPRelation(){codeType=(OpCodeType)60000,typeRequest=typeof(IMessage),typeResponse=typeof(SCEmptyCoerceRenewal)},
			
        };

        public static OpCodeType GetOpCodeTypeByRequest<T>() where T:IMessage
        {
            System.Type typeRequest = typeof(T);
            OPRelation opRelation=listOpRelation.Find((obj) => obj.typeRequest == typeRequest);
            return opRelation.codeType;
        }

        public static IMessage GetRealMessageByOpCodeType(OpCodeType codeType, byte[] data)
        {
            if ((int)codeType == 1)
            {
                return ProtoSerAndUnSer.UnSerialize<SCLinkInfo>(data);
            }
			if ((int)codeType == 2)
            {
                return ProtoSerAndUnSer.UnSerialize<SCUserInfo>(data);
            }
			if ((int)codeType == 3)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpySetBasicsInfo>(data);
            }
			if ((int)codeType == 5)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendList>(data);
            }
			if ((int)codeType == 6)
            {
                return ProtoSerAndUnSer.UnSerialize<SCApplyList>(data);
            }
			if ((int)codeType == 7)
            {
                return ProtoSerAndUnSer.UnSerialize<SCRecommendList>(data);
            }
			if ((int)codeType == 8)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyApply>(data);
            }
			if ((int)codeType == 9)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyAccept>(data);
            }
			if ((int)codeType == 10)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyRepluse>(data);
            }
			if ((int)codeType == 11)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyDelFriend>(data);
            }
			if ((int)codeType == 12)
            {
                return ProtoSerAndUnSer.UnSerialize<SCSearch>(data);
            }
			if ((int)codeType == 13)
            {
                return ProtoSerAndUnSer.UnSerialize<SCSearch>(data);
            }
			if ((int)codeType == 14)
            {
                return ProtoSerAndUnSer.UnSerialize<SCDiceResult>(data);
            }
			if ((int)codeType == 15)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyRichManFinish>(data);
            }
			if ((int)codeType == 16)
            {
                return ProtoSerAndUnSer.UnSerialize<SCLucky>(data);
            }
			if ((int)codeType == 17)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyCropSpeed>(data);
            }
			if ((int)codeType == 18)
            {
                return ProtoSerAndUnSer.UnSerialize<SCBuyProp>(data);
            }
			if ((int)codeType == 19)
            {
                return ProtoSerAndUnSer.UnSerialize<SCDrag>(data);
            }
			if ((int)codeType == 20)
            {
                return ProtoSerAndUnSer.UnSerialize<SCManorData>(data);
            }
			if ((int)codeType == 21)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptFertilizer>(data);
            }
			if ((int)codeType == 22)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptEradicate>(data);
            }
			if ((int)codeType == 23)
            {
                return ProtoSerAndUnSer.UnSerialize<SCPlantResult>(data);
            }
			if ((int)codeType == 24)
            {
                return ProtoSerAndUnSer.UnSerialize<SCHarvestData>(data);
            }
			if ((int)codeType == 25)
            {
                return ProtoSerAndUnSer.UnSerialize<SCStealData>(data);
            }
			if ((int)codeType == 26)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptChangeLocationData>(data);
            }
			if ((int)codeType == 27)
            {
                return ProtoSerAndUnSer.UnSerialize<SCGoodSellData>(data);
            }
			if ((int)codeType == 28)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptGoodLock>(data);
            }
			if ((int)codeType == 29)
            {
                return ProtoSerAndUnSer.UnSerialize<SCManorFriendData>(data);
            }
			if ((int)codeType == 30)
            {
                return ProtoSerAndUnSer.UnSerialize<SCUnlockArea>(data);
            }
			if ((int)codeType == 31)
            {
                return ProtoSerAndUnSer.UnSerialize<CSRoleSInfo>(data);
            }
			if ((int)codeType == 32)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptChoiceRole>(data);
            }
			if ((int)codeType == 33)
            {
                return ProtoSerAndUnSer.UnSerialize<SCBuyProp>(data);
            }
			if ((int)codeType == 34)
            {
                return ProtoSerAndUnSer.UnSerialize<SCPromotionsGoodsInfo>(data);
            }
			if ((int)codeType == 35)
            {
                return ProtoSerAndUnSer.UnSerialize<SCGoldJewelBuy>(data);
            }
			if ((int)codeType == 36)
            {
                return ProtoSerAndUnSer.UnSerialize<SCBuySection>(data);
            }
			if ((int)codeType == 37)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyClearance>(data);
            }
			if ((int)codeType == 38)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyOrnamentalRecycle>(data);
            }
			if ((int)codeType == 39)
            {
                return ProtoSerAndUnSer.UnSerialize<SCTreasureChestResult>(data);
            }
			if ((int)codeType == 40)
            {
                return ProtoSerAndUnSer.UnSerialize<SCGetTreasureChestAwardResult>(data);
            }
			if ((int)codeType == 43)
            {
                return ProtoSerAndUnSer.UnSerialize<SCBuyExtraStory>(data);
            }
			if ((int)codeType == 44)
            {
                return ProtoSerAndUnSer.UnSerialize<SCMailInfo>(data);
            }
			if ((int)codeType == 45)
            {
                return ProtoSerAndUnSer.UnSerialize<SCAccessoryInWarehouse>(data);
            }
			if ((int)codeType == 46)
            {
                return ProtoSerAndUnSer.UnSerialize<SCAccessory>(data);
            }
			if ((int)codeType == 47)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEverydayAward>(data);
            }
			if ((int)codeType == 49)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendStealInfo>(data);
            }
			if ((int)codeType == 50)
            {
                return ProtoSerAndUnSer.UnSerialize<SCCurrentExperience>(data);
            }
			if ((int)codeType == 53)
            {
                return ProtoSerAndUnSer.UnSerialize<SCWorkShedSpeedUp>(data);
            }
			if ((int)codeType == 54)
            {
                return ProtoSerAndUnSer.UnSerialize<SCTreasureChestUnlockInfo>(data);
            }
			if ((int)codeType == 55)
            {
                return ProtoSerAndUnSer.UnSerialize<SCTreasureChestSpeed>(data);
            }
			if ((int)codeType == 56)
            {
                return ProtoSerAndUnSer.UnSerialize<SCUseWarehouseGoods>(data);
            }
			if ((int)codeType == 57)
            {
                return ProtoSerAndUnSer.UnSerialize<SCManorLogs>(data);
            }
			if ((int)codeType == 58)
            {
                return ProtoSerAndUnSer.UnSerialize<SCPurchaseRole>(data);
            }
			if ((int)codeType == 59)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptySwitchoverRole>(data);
            }
			if ((int)codeType == 60)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptySwitchoverCostume>(data);
            }
			if ((int)codeType == 61)
            {
                return ProtoSerAndUnSer.UnSerialize<SCOneselfMarmot>(data);
            }
			if ((int)codeType == 62)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendMarmot>(data);
            }
			if ((int)codeType == 63)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyAdvExtraStory>(data);
            }
			if ((int)codeType == 64)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyWorldChat>(data);
            }
			if ((int)codeType == 65)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyPrivateChat>(data);
            }
			if ((int)codeType == 66)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEntranceRoom>(data);
            }
			if ((int)codeType == 67)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyGuessing>(data);
            }
			if ((int)codeType == 68)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyChannelChat>(data);
            }
			if ((int)codeType == 69)
            {
                return ProtoSerAndUnSer.UnSerialize<SCRoomListInfo>(data);
            }
			if ((int)codeType == 70)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyLoginGetFavorable>(data);
            }
			if ((int)codeType == 71)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptySendNPCGift>(data);
            }
			if ((int)codeType == 72)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpySectionAddFavorable>(data);
            }
			if ((int)codeType == 74)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFavorableInfo>(data);
            }
			if ((int)codeType == 75)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyMoveLocation>(data);
            }
			if ((int)codeType == 76)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyDepartureRoom>(data);
            }
			if ((int)codeType == 78)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyMotion>(data);
            }
			if ((int)codeType == 79)
            {
                return ProtoSerAndUnSer.UnSerialize<SCMarmotAwardInfo>(data);
            }
			if ((int)codeType == 80)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyChangeMailState>(data);
            }
			if ((int)codeType == 81)
            {
                return ProtoSerAndUnSer.UnSerialize<SCBuyProp>(data);
            }
			if ((int)codeType == 82)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyHeartBeat>(data);
            }
			if ((int)codeType == 83)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyAccumulateSignIn>(data);
            }
			if ((int)codeType == 84)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptySaveGuidance>(data);
            }
			if ((int)codeType == 87)
            {
                return ProtoSerAndUnSer.UnSerialize<SCManorGuidance>(data);
            }
			if ((int)codeType == 88)
            {
                return ProtoSerAndUnSer.UnSerialize<SCStealManorGuidance>(data);
            }
			if ((int)codeType == 91)
            {
                return ProtoSerAndUnSer.UnSerialize<SCGetTaskInfo>(data);
            }
			if ((int)codeType == 92)
            {
                return ProtoSerAndUnSer.UnSerialize<SCGetTaskAward>(data);
            }
			if ((int)codeType == 93)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyLogoutAccount>(data);
            }
			if ((int)codeType == 94)
            {
                return ProtoSerAndUnSer.UnSerialize<SCCdkAward>(data);
            }
			if ((int)codeType == 95)
            {
                return ProtoSerAndUnSer.UnSerialize<SCDealInfo>(data);
            }
			if ((int)codeType == 96)
            {
                return ProtoSerAndUnSer.UnSerialize<SCSubmintDeal>(data);
            }
			if ((int)codeType == 97)
            {
                return ProtoSerAndUnSer.UnSerialize<SCRefreshDeal>(data);
            }
			if ((int)codeType == 98)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyAdvSkipDeal>(data);
            }
			if ((int)codeType == 99)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyEntranceSection>(data);
            }
			if ((int)codeType == 100)
            {
                return ProtoSerAndUnSer.UnSerialize<SCCropMature>(data);
            }
			if ((int)codeType == 101)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyManorDecorateRotate>(data);
            }
			if ((int)codeType == 102)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyLike>(data);
            }
			if ((int)codeType == 103)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendCostume>(data);
            }
			if ((int)codeType == 104)
            {
                return ProtoSerAndUnSer.UnSerialize<SCOnceWatering>(data);
            }
			if ((int)codeType == 105)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyDerectUp>(data);
            }
			if ((int)codeType == 50002)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendApplyPushMsg>(data);
            }
			if ((int)codeType == 50003)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendAcceptPushMsg>(data);
            }
			if ((int)codeType == 50004)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendRepulsePushMsg>(data);
            }
			if ((int)codeType == 50005)
            {
                return ProtoSerAndUnSer.UnSerialize<SCFriendDeletePushMsg>(data);
            }
			if ((int)codeType == 50006)
            {
                return ProtoSerAndUnSer.UnSerialize<SCSendMailPushMsg>(data);
            }
			if ((int)codeType == 50009)
            {
                return ProtoSerAndUnSer.UnSerialize<SCNotePushMess>(data);
            }
			if ((int)codeType == 50010)
            {
                return ProtoSerAndUnSer.UnSerialize<SCNotePushMess>(data);
            }
			if ((int)codeType == 50011)
            {
                return ProtoSerAndUnSer.UnSerialize<SCChat>(data);
            }
			if ((int)codeType == 50012)
            {
                return ProtoSerAndUnSer.UnSerialize<SCChat>(data);
            }
			if ((int)codeType == 50013)
            {
                return ProtoSerAndUnSer.UnSerialize<SCChat>(data);
            }
			if ((int)codeType == 50014)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEntranceRoomInfo>(data);
            }
			if ((int)codeType == 50015)
            {
                return ProtoSerAndUnSer.UnSerialize<SCMoveLocation>(data);
            }
			if ((int)codeType == 50016)
            {
                return ProtoSerAndUnSer.UnSerialize<SCpushGuessingInfo>(data);
            }
			if ((int)codeType == 50017)
            {
                return ProtoSerAndUnSer.UnSerialize<SCpushMotion>(data);
            }
			if ((int)codeType == 50018)
            {
                return ProtoSerAndUnSer.UnSerialize<SCDepartureRoom>(data);
            }
			if ((int)codeType == 50019)
            {
                return ProtoSerAndUnSer.UnSerialize<SCActivityFinish>(data);
            }
			if ((int)codeType == 50020)
            {
                return ProtoSerAndUnSer.UnSerialize<SCRepetitionRegister>(data);
            }
			if ((int)codeType == 50021)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmtpyMailPushMsg>(data);
            }
			if ((int)codeType == 60000)
            {
                return ProtoSerAndUnSer.UnSerialize<SCEmptyCoerceRenewal>(data);
            }
			
            return null;
        }
    }
}