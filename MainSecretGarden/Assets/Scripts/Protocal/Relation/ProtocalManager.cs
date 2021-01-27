using System;

namespace Game.Protocal
{
    public class ProtocalManager
    {

        private static ProtocalManager _instance;
        private ProtocalManager() { }

        public static ProtocalManager Instance()
        {
            if (_instance == null)
            {
                _instance = new ProtocalManager();
            }
            return _instance;
        }

        public void SendCSLinkInfo(CSLinkInfo cslinkinfo, Action<SCLinkInfo> ResponseSCLinkInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSLinkInfo> ();
            ProtoSendMethod.BusinessRequest<SCLinkInfo>(cslinkinfo, opCodeType, ResponseSCLinkInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyAccountInfo(CSEmptyAccountInfo csemptyaccountinfo, Action<SCUserInfo> ResponseSCUserInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyAccountInfo> ();
            ProtoSendMethod.BusinessRequest<SCUserInfo>(csemptyaccountinfo, opCodeType, ResponseSCUserInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSetBasicsInfo(CSSetBasicsInfo cssetbasicsinfo, Action<SCEmtpySetBasicsInfo> ResponseSCEmtpySetBasicsInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSetBasicsInfo> ();
            ProtoSendMethod.BusinessRequest<SCEmtpySetBasicsInfo>(cssetbasicsinfo, opCodeType, ResponseSCEmtpySetBasicsInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptySCFriendList(CSEmptySCFriendList csemptyscfriendlist, Action<SCFriendList> ResponseSCFriendListCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptySCFriendList> ();
            ProtoSendMethod.BusinessRequest<SCFriendList>(csemptyscfriendlist, opCodeType, ResponseSCFriendListCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyApplyList(CSEmptyApplyList csemptyapplylist, Action<SCApplyList> ResponseSCApplyListCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyApplyList> ();
            ProtoSendMethod.BusinessRequest<SCApplyList>(csemptyapplylist, opCodeType, ResponseSCApplyListCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyRecommendList(CSEmptyRecommendList csemptyrecommendlist, Action<SCRecommendList> ResponseSCRecommendListCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyRecommendList> ();
            ProtoSendMethod.BusinessRequest<SCRecommendList>(csemptyrecommendlist, opCodeType, ResponseSCRecommendListCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSApply(CSApply csapply, Action<SCEmptyApply> ResponseSCEmptyApplyCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSApply> ();
            ProtoSendMethod.BusinessRequest<SCEmptyApply>(csapply, opCodeType, ResponseSCEmptyApplyCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAccept(CSAccept csaccept, Action<SCEmptyAccept> ResponseSCEmptyAcceptCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAccept> ();
            ProtoSendMethod.BusinessRequest<SCEmptyAccept>(csaccept, opCodeType, ResponseSCEmptyAcceptCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSRepulse(CSRepulse csrepulse, Action<SCEmptyRepluse> ResponseSCEmptyRepluseCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSRepulse> ();
            ProtoSendMethod.BusinessRequest<SCEmptyRepluse>(csrepulse, opCodeType, ResponseSCEmptyRepluseCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSDelFriend(CSDelFriend csdelfriend, Action<SCEmtpyDelFriend> ResponseSCEmtpyDelFriendCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSDelFriend> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyDelFriend>(csdelfriend, opCodeType, ResponseSCEmtpyDelFriendCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSearch(CSSearch cssearch, Action<SCSearch> ResponseSCSearchCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSearch> ();
            ProtoSendMethod.BusinessRequest<SCSearch>(cssearch, opCodeType, ResponseSCSearchCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEnterMap(CSEnterMap csentermap, Action<SCSearch> ResponseSCSearchCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEnterMap> ();
            ProtoSendMethod.BusinessRequest<SCSearch>(csentermap, opCodeType, ResponseSCSearchCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSDice(CSDice csdice, Action<SCDiceResult> ResponseSCDiceResultCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSDice> ();
            ProtoSendMethod.BusinessRequest<SCDiceResult>(csdice, opCodeType, ResponseSCDiceResultCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyRichManFinish(CSEmptyRichManFinish csemptyrichmanfinish, Action<SCEmptyRichManFinish> ResponseSCEmptyRichManFinishCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyRichManFinish> ();
            ProtoSendMethod.BusinessRequest<SCEmptyRichManFinish>(csemptyrichmanfinish, opCodeType, ResponseSCEmptyRichManFinishCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSLucky(CSLucky cslucky, Action<SCLucky> ResponseSCLuckyCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSLucky> ();
            ProtoSendMethod.BusinessRequest<SCLucky>(cslucky, opCodeType, ResponseSCLuckyCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyCropSpeed(CSEmptyCropSpeed csemptycropspeed, Action<SCEmptyCropSpeed> ResponseSCEmptyCropSpeedCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyCropSpeed> ();
            ProtoSendMethod.BusinessRequest<SCEmptyCropSpeed>(csemptycropspeed, opCodeType, ResponseSCEmptyCropSpeedCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSBuyProp(CSBuyProp csbuyprop, Action<SCBuyProp> ResponseSCBuyPropCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSBuyProp> ();
            ProtoSendMethod.BusinessRequest<SCBuyProp>(csbuyprop, opCodeType, ResponseSCBuyPropCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSDrag(CSDrag csdrag, Action<SCDrag> ResponseSCDragCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSDrag> ();
            ProtoSendMethod.BusinessRequest<SCDrag>(csdrag, opCodeType, ResponseSCDragCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyManorInfo(CSEmptyManorInfo csemptymanorinfo, Action<SCManorData> ResponseSCManorDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyManorInfo> ();
            ProtoSendMethod.BusinessRequest<SCManorData>(csemptymanorinfo, opCodeType, ResponseSCManorDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSFertilizer(CSFertilizer csfertilizer, Action<SCEmptFertilizer> ResponseSCEmptFertilizerCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSFertilizer> ();
            ProtoSendMethod.BusinessRequest<SCEmptFertilizer>(csfertilizer, opCodeType, ResponseSCEmptFertilizerCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEradicate(CSEradicate cseradicate, Action<SCEmptEradicate> ResponseSCEmptEradicateCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEradicate> ();
            ProtoSendMethod.BusinessRequest<SCEmptEradicate>(cseradicate, opCodeType, ResponseSCEmptEradicateCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSPlantData(CSPlantData csplantdata, Action<SCPlantResult> ResponseSCPlantResultCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSPlantData> ();
            ProtoSendMethod.BusinessRequest<SCPlantResult>(csplantdata, opCodeType, ResponseSCPlantResultCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSHarvestData(CSHarvestData csharvestdata, Action<SCHarvestData> ResponseSCHarvestDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSHarvestData> ();
            ProtoSendMethod.BusinessRequest<SCHarvestData>(csharvestdata, opCodeType, ResponseSCHarvestDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSStealData(CSStealData csstealdata, Action<SCStealData> ResponseSCStealDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSStealData> ();
            ProtoSendMethod.BusinessRequest<SCStealData>(csstealdata, opCodeType, ResponseSCStealDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSChangeLocation(CSChangeLocation cschangelocation, Action<SCEmptChangeLocationData> ResponseSCEmptChangeLocationDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSChangeLocation> ();
            ProtoSendMethod.BusinessRequest<SCEmptChangeLocationData>(cschangelocation, opCodeType, ResponseSCEmptChangeLocationDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGoodSellData(CSGoodSellData csgoodselldata, Action<SCGoodSellData> ResponseSCGoodSellDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGoodSellData> ();
            ProtoSendMethod.BusinessRequest<SCGoodSellData>(csgoodselldata, opCodeType, ResponseSCGoodSellDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGoodLock(CSGoodLock csgoodlock, Action<SCEmptGoodLock> ResponseSCEmptGoodLockCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGoodLock> ();
            ProtoSendMethod.BusinessRequest<SCEmptGoodLock>(csgoodlock, opCodeType, ResponseSCEmptGoodLockCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSQueryOther(CSQueryOther csqueryother, Action<SCManorFriendData> ResponseSCManorFriendDataCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSQueryOther> ();
            ProtoSendMethod.BusinessRequest<SCManorFriendData>(csqueryother, opCodeType, ResponseSCManorFriendDataCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSUnlockArea(CSUnlockArea csunlockarea, Action<SCUnlockArea> ResponseSCUnlockAreaCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSUnlockArea> ();
            ProtoSendMethod.BusinessRequest<SCUnlockArea>(csunlockarea, opCodeType, ResponseSCUnlockAreaCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyRoleList(CSEmptyRoleList csemptyrolelist, Action<CSRoleSInfo> ResponseCSRoleSInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyRoleList> ();
            ProtoSendMethod.BusinessRequest<CSRoleSInfo>(csemptyrolelist, opCodeType, ResponseCSRoleSInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSChoiceRole(CSChoiceRole cschoicerole, Action<SCEmptChoiceRole> ResponseSCEmptChoiceRoleCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSChoiceRole> ();
            ProtoSendMethod.BusinessRequest<SCEmptChoiceRole>(cschoicerole, opCodeType, ResponseSCEmptChoiceRoleCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSRoleBuy(CSRoleBuy csrolebuy, Action<SCBuyProp> ResponseSCBuyPropCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSRoleBuy> ();
            ProtoSendMethod.BusinessRequest<SCBuyProp>(csrolebuy, opCodeType, ResponseSCBuyPropCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyPromotionsGoodsInfo(CSEmptyPromotionsGoodsInfo csemptypromotionsgoodsinfo, Action<SCPromotionsGoodsInfo> ResponseSCPromotionsGoodsInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyPromotionsGoodsInfo> ();
            ProtoSendMethod.BusinessRequest<SCPromotionsGoodsInfo>(csemptypromotionsgoodsinfo, opCodeType, ResponseSCPromotionsGoodsInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGoldJewelBuy(CSGoldJewelBuy csgoldjewelbuy, Action<SCGoldJewelBuy> ResponseSCGoldJewelBuyCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGoldJewelBuy> ();
            ProtoSendMethod.BusinessRequest<SCGoldJewelBuy>(csgoldjewelbuy, opCodeType, ResponseSCGoldJewelBuyCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSBuySection(CSBuySection csbuysection, Action<SCBuySection> ResponseSCBuySectionCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSBuySection> ();
            ProtoSendMethod.BusinessRequest<SCBuySection>(csbuysection, opCodeType, ResponseSCBuySectionCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSClearance(CSClearance csclearance, Action<SCEmptyClearance> ResponseSCEmptyClearanceCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSClearance> ();
            ProtoSendMethod.BusinessRequest<SCEmptyClearance>(csclearance, opCodeType, ResponseSCEmptyClearanceCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSOrnamentalRecycle(CSOrnamentalRecycle csornamentalrecycle, Action<SCEmptyOrnamentalRecycle> ResponseSCEmptyOrnamentalRecycleCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSOrnamentalRecycle> ();
            ProtoSendMethod.BusinessRequest<SCEmptyOrnamentalRecycle>(csornamentalrecycle, opCodeType, ResponseSCEmptyOrnamentalRecycleCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSTreasureChest(CSTreasureChest cstreasurechest, Action<SCTreasureChestResult> ResponseSCTreasureChestResultCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSTreasureChest> ();
            ProtoSendMethod.BusinessRequest<SCTreasureChestResult>(cstreasurechest, opCodeType, ResponseSCTreasureChestResultCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGetTreasureChestAward(CSGetTreasureChestAward csgettreasurechestaward, Action<SCGetTreasureChestAwardResult> ResponseSCGetTreasureChestAwardResultCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGetTreasureChestAward> ();
            ProtoSendMethod.BusinessRequest<SCGetTreasureChestAwardResult>(csgettreasurechestaward, opCodeType, ResponseSCGetTreasureChestAwardResultCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSBuyExtraStory(CSBuyExtraStory csbuyextrastory, Action<SCBuyExtraStory> ResponseSCBuyExtraStoryCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSBuyExtraStory> ();
            ProtoSendMethod.BusinessRequest<SCBuyExtraStory>(csbuyextrastory, opCodeType, ResponseSCBuyExtraStoryCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyMail(CSEmptyMail csemptymail, Action<SCMailInfo> ResponseSCMailInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyMail> ();
            ProtoSendMethod.BusinessRequest<SCMailInfo>(csemptymail, opCodeType, ResponseSCMailInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAccessoryInWarehouse(CSAccessoryInWarehouse csaccessoryinwarehouse, Action<SCAccessoryInWarehouse> ResponseSCAccessoryInWarehouseCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAccessoryInWarehouse> ();
            ProtoSendMethod.BusinessRequest<SCAccessoryInWarehouse>(csaccessoryinwarehouse, opCodeType, ResponseSCAccessoryInWarehouseCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAccessoryInfo(CSAccessoryInfo csaccessoryinfo, Action<SCAccessory> ResponseSCAccessoryCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAccessoryInfo> ();
            ProtoSendMethod.BusinessRequest<SCAccessory>(csaccessoryinfo, opCodeType, ResponseSCAccessoryCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptySignIn(CSEmptySignIn csemptysignin, Action<SCEverydayAward> ResponseSCEverydayAwardCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptySignIn> ();
            ProtoSendMethod.BusinessRequest<SCEverydayAward>(csemptysignin, opCodeType, ResponseSCEverydayAwardCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyFriendSteal(CSEmptyFriendSteal csemptyfriendsteal, Action<SCFriendStealInfo> ResponseSCFriendStealInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyFriendSteal> ();
            ProtoSendMethod.BusinessRequest<SCFriendStealInfo>(csemptyfriendsteal, opCodeType, ResponseSCFriendStealInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyUserUpGrade(CSEmptyUserUpGrade csemptyuserupgrade, Action<SCCurrentExperience> ResponseSCCurrentExperienceCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyUserUpGrade> ();
            ProtoSendMethod.BusinessRequest<SCCurrentExperience>(csemptyuserupgrade, opCodeType, ResponseSCCurrentExperienceCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSWorkShedSpeedUp(CSWorkShedSpeedUp csworkshedspeedup, Action<SCWorkShedSpeedUp> ResponseSCWorkShedSpeedUpCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSWorkShedSpeedUp> ();
            ProtoSendMethod.BusinessRequest<SCWorkShedSpeedUp>(csworkshedspeedup, opCodeType, ResponseSCWorkShedSpeedUpCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyWarehouseUnlockInfo(CSEmptyWarehouseUnlockInfo csemptywarehouseunlockinfo, Action<SCTreasureChestUnlockInfo> ResponseSCTreasureChestUnlockInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyWarehouseUnlockInfo> ();
            ProtoSendMethod.BusinessRequest<SCTreasureChestUnlockInfo>(csemptywarehouseunlockinfo, opCodeType, ResponseSCTreasureChestUnlockInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSTreasureChestSpeed(CSTreasureChestSpeed cstreasurechestspeed, Action<SCTreasureChestSpeed> ResponseSCTreasureChestSpeedCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSTreasureChestSpeed> ();
            ProtoSendMethod.BusinessRequest<SCTreasureChestSpeed>(cstreasurechestspeed, opCodeType, ResponseSCTreasureChestSpeedCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSUseWarehouseGoods(CSUseWarehouseGoods csusewarehousegoods, Action<SCUseWarehouseGoods> ResponseSCUseWarehouseGoodsCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSUseWarehouseGoods> ();
            ProtoSendMethod.BusinessRequest<SCUseWarehouseGoods>(csusewarehousegoods, opCodeType, ResponseSCUseWarehouseGoodsCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyManorLogs(CSEmptyManorLogs csemptymanorlogs, Action<SCManorLogs> ResponseSCManorLogsCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyManorLogs> ();
            ProtoSendMethod.BusinessRequest<SCManorLogs>(csemptymanorlogs, opCodeType, ResponseSCManorLogsCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSPurchaseRole(CSPurchaseRole cspurchaserole, Action<SCPurchaseRole> ResponseSCPurchaseRoleCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSPurchaseRole> ();
            ProtoSendMethod.BusinessRequest<SCPurchaseRole>(cspurchaserole, opCodeType, ResponseSCPurchaseRoleCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSwitchoverRole(CSSwitchoverRole csswitchoverrole, Action<SCEmptySwitchoverRole> ResponseSCEmptySwitchoverRoleCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSwitchoverRole> ();
            ProtoSendMethod.BusinessRequest<SCEmptySwitchoverRole>(csswitchoverrole, opCodeType, ResponseSCEmptySwitchoverRoleCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSwitchoverCostume(CSSwitchoverCostume csswitchovercostume, Action<SCEmptySwitchoverCostume> ResponseSCEmptySwitchoverCostumeCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSwitchoverCostume> ();
            ProtoSendMethod.BusinessRequest<SCEmptySwitchoverCostume>(csswitchovercostume, opCodeType, ResponseSCEmptySwitchoverCostumeCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSOneselfMarmot(CSOneselfMarmot csoneselfmarmot, Action<SCOneselfMarmot> ResponseSCOneselfMarmotCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSOneselfMarmot> ();
            ProtoSendMethod.BusinessRequest<SCOneselfMarmot>(csoneselfmarmot, opCodeType, ResponseSCOneselfMarmotCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSFriendMarmot(CSFriendMarmot csfriendmarmot, Action<SCFriendMarmot> ResponseSCFriendMarmotCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSFriendMarmot> ();
            ProtoSendMethod.BusinessRequest<SCFriendMarmot>(csfriendmarmot, opCodeType, ResponseSCFriendMarmotCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAdvExtraStory(CSAdvExtraStory csadvextrastory, Action<SCEmtpyAdvExtraStory> ResponseSCEmtpyAdvExtraStoryCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAdvExtraStory> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyAdvExtraStory>(csadvextrastory, opCodeType, ResponseSCEmtpyAdvExtraStoryCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSWorldChat(CSWorldChat csworldchat, Action<SCEmptyWorldChat> ResponseSCEmptyWorldChatCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSWorldChat> ();
            ProtoSendMethod.BusinessRequest<SCEmptyWorldChat>(csworldchat, opCodeType, ResponseSCEmptyWorldChatCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSPrivateChat(CSPrivateChat csprivatechat, Action<SCEmptyPrivateChat> ResponseSCEmptyPrivateChatCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSPrivateChat> ();
            ProtoSendMethod.BusinessRequest<SCEmptyPrivateChat>(csprivatechat, opCodeType, ResponseSCEmptyPrivateChatCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEntranceRoom(CSEntranceRoom csentranceroom, Action<SCEntranceRoom> ResponseSCEntranceRoomCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEntranceRoom> ();
            ProtoSendMethod.BusinessRequest<SCEntranceRoom>(csentranceroom, opCodeType, ResponseSCEntranceRoomCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGuessing(CSGuessing csguessing, Action<SCEmtpyGuessing> ResponseSCEmtpyGuessingCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGuessing> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyGuessing>(csguessing, opCodeType, ResponseSCEmtpyGuessingCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSChannelChat(CSChannelChat cschannelchat, Action<SCEmtpyChannelChat> ResponseSCEmtpyChannelChatCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSChannelChat> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyChannelChat>(cschannelchat, opCodeType, ResponseSCEmtpyChannelChatCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyRoomList(CSEmptyRoomList csemptyroomlist, Action<SCRoomListInfo> ResponseSCRoomListInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyRoomList> ();
            ProtoSendMethod.BusinessRequest<SCRoomListInfo>(csemptyroomlist, opCodeType, ResponseSCRoomListInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGetLoginFavorable(CSGetLoginFavorable csgetloginfavorable, Action<SCEmptyLoginGetFavorable> ResponseSCEmptyLoginGetFavorableCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGetLoginFavorable> ();
            ProtoSendMethod.BusinessRequest<SCEmptyLoginGetFavorable>(csgetloginfavorable, opCodeType, ResponseSCEmptyLoginGetFavorableCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSendNPCGift(CSSendNPCGift cssendnpcgift, Action<SCEmptySendNPCGift> ResponseSCEmptySendNPCGiftCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSendNPCGift> ();
            ProtoSendMethod.BusinessRequest<SCEmptySendNPCGift>(cssendnpcgift, opCodeType, ResponseSCEmptySendNPCGiftCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSectionAddFavorable(CSSectionAddFavorable cssectionaddfavorable, Action<SCEmtpySectionAddFavorable> ResponseSCEmtpySectionAddFavorableCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSectionAddFavorable> ();
            ProtoSendMethod.BusinessRequest<SCEmtpySectionAddFavorable>(cssectionaddfavorable, opCodeType, ResponseSCEmtpySectionAddFavorableCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyFavorableInfo(CSEmptyFavorableInfo csemptyfavorableinfo, Action<SCFavorableInfo> ResponseSCFavorableInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyFavorableInfo> ();
            ProtoSendMethod.BusinessRequest<SCFavorableInfo>(csemptyfavorableinfo, opCodeType, ResponseSCFavorableInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSMoveLocation(CSMoveLocation csmovelocation, Action<SCEmtpyMoveLocation> ResponseSCEmtpyMoveLocationCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSMoveLocation> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyMoveLocation>(csmovelocation, opCodeType, ResponseSCEmtpyMoveLocationCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyDepartureRoom(CSEmptyDepartureRoom csemptydepartureroom, Action<SCEmptyDepartureRoom> ResponseSCEmptyDepartureRoomCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyDepartureRoom> ();
            ProtoSendMethod.BusinessRequest<SCEmptyDepartureRoom>(csemptydepartureroom, opCodeType, ResponseSCEmptyDepartureRoomCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSMotion(CSMotion csmotion, Action<SCEmptyMotion> ResponseSCEmptyMotionCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSMotion> ();
            ProtoSendMethod.BusinessRequest<SCEmptyMotion>(csmotion, opCodeType, ResponseSCEmptyMotionCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyMarmotAwardInfo(CSEmptyMarmotAwardInfo csemptymarmotawardinfo, Action<SCMarmotAwardInfo> ResponseSCMarmotAwardInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyMarmotAwardInfo> ();
            ProtoSendMethod.BusinessRequest<SCMarmotAwardInfo>(csemptymarmotawardinfo, opCodeType, ResponseSCMarmotAwardInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSChangeMailState(CSChangeMailState cschangemailstate, Action<SCEmtpyChangeMailState> ResponseSCEmtpyChangeMailStateCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSChangeMailState> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyChangeMailState>(cschangemailstate, opCodeType, ResponseSCEmtpyChangeMailStateCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSBuyPropInfo(CSBuyPropInfo csbuypropinfo, Action<SCBuyProp> ResponseSCBuyPropCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSBuyPropInfo> ();
            ProtoSendMethod.BusinessRequest<SCBuyProp>(csbuypropinfo, opCodeType, ResponseSCBuyPropCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyHeartBeat(CSEmptyHeartBeat csemptyheartbeat, Action<SCEmtpyHeartBeat> ResponseSCEmtpyHeartBeatCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyHeartBeat> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyHeartBeat>(csemptyheartbeat, opCodeType, ResponseSCEmtpyHeartBeatCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAccumulateSignIn(CSAccumulateSignIn csaccumulatesignin, Action<SCEmtpyAccumulateSignIn> ResponseSCEmtpyAccumulateSignInCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAccumulateSignIn> ();
            ProtoSendMethod.BusinessRequest<SCEmtpyAccumulateSignIn>(csaccumulatesignin, opCodeType, ResponseSCEmtpyAccumulateSignInCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSaveGuidance(CSSaveGuidance cssaveguidance, Action<SCEmptySaveGuidance> ResponseSCEmptySaveGuidanceCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSaveGuidance> ();
            ProtoSendMethod.BusinessRequest<SCEmptySaveGuidance>(cssaveguidance, opCodeType, ResponseSCEmptySaveGuidanceCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyManorGuidance(CSEmptyManorGuidance csemptymanorguidance, Action<SCManorGuidance> ResponseSCManorGuidanceCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyManorGuidance> ();
            ProtoSendMethod.BusinessRequest<SCManorGuidance>(csemptymanorguidance, opCodeType, ResponseSCManorGuidanceCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyStealManorGuidance(CSEmptyStealManorGuidance csemptystealmanorguidance, Action<SCStealManorGuidance> ResponseSCStealManorGuidanceCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyStealManorGuidance> ();
            ProtoSendMethod.BusinessRequest<SCStealManorGuidance>(csemptystealmanorguidance, opCodeType, ResponseSCStealManorGuidanceCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyGetTaskInfo(CSEmptyGetTaskInfo csemptygettaskinfo, Action<SCGetTaskInfo> ResponseSCGetTaskInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyGetTaskInfo> ();
            ProtoSendMethod.BusinessRequest<SCGetTaskInfo>(csemptygettaskinfo, opCodeType, ResponseSCGetTaskInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSGetTaskAward(CSGetTaskAward csgettaskaward, Action<SCGetTaskAward> ResponseSCGetTaskAwardCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSGetTaskAward> ();
            ProtoSendMethod.BusinessRequest<SCGetTaskAward>(csgettaskaward, opCodeType, ResponseSCGetTaskAwardCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyLogoutAccount(CSEmptyLogoutAccount csemptylogoutaccount, Action<SCEmptyLogoutAccount> ResponseSCEmptyLogoutAccountCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyLogoutAccount> ();
            ProtoSendMethod.BusinessRequest<SCEmptyLogoutAccount>(csemptylogoutaccount, opCodeType, ResponseSCEmptyLogoutAccountCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSCdkAward(CSCdkAward cscdkaward, Action<SCCdkAward> ResponseSCCdkAwardCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSCdkAward> ();
            ProtoSendMethod.BusinessRequest<SCCdkAward>(cscdkaward, opCodeType, ResponseSCCdkAwardCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyDealInfo(CSEmptyDealInfo csemptydealinfo, Action<SCDealInfo> ResponseSCDealInfoCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyDealInfo> ();
            ProtoSendMethod.BusinessRequest<SCDealInfo>(csemptydealinfo, opCodeType, ResponseSCDealInfoCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSSubmintDeal(CSSubmintDeal cssubmintdeal, Action<SCSubmintDeal> ResponseSCSubmintDealCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSSubmintDeal> ();
            ProtoSendMethod.BusinessRequest<SCSubmintDeal>(cssubmintdeal, opCodeType, ResponseSCSubmintDealCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSRefreshDeal(CSRefreshDeal csrefreshdeal, Action<SCRefreshDeal> ResponseSCRefreshDealCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSRefreshDeal> ();
            ProtoSendMethod.BusinessRequest<SCRefreshDeal>(csrefreshdeal, opCodeType, ResponseSCRefreshDealCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSAdvSkipDeal(CSAdvSkipDeal csadvskipdeal, Action<SCEmptyAdvSkipDeal> ResponseSCEmptyAdvSkipDealCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSAdvSkipDeal> ();
            ProtoSendMethod.BusinessRequest<SCEmptyAdvSkipDeal>(csadvskipdeal, opCodeType, ResponseSCEmptyAdvSkipDealCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEntranceSection(CSEntranceSection csentrancesection, Action<SCEmptyEntranceSection> ResponseSCEmptyEntranceSectionCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEntranceSection> ();
            ProtoSendMethod.BusinessRequest<SCEmptyEntranceSection>(csentrancesection, opCodeType, ResponseSCEmptyEntranceSectionCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyCropMature(CSEmptyCropMature csemptycropmature, Action<SCCropMature> ResponseSCCropMatureCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyCropMature> ();
            ProtoSendMethod.BusinessRequest<SCCropMature>(csemptycropmature, opCodeType, ResponseSCCropMatureCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSManorDecorateRotate(CSManorDecorateRotate csmanordecoraterotate, Action<SCEmptyManorDecorateRotate> ResponseSCEmptyManorDecorateRotateCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSManorDecorateRotate> ();
            ProtoSendMethod.BusinessRequest<SCEmptyManorDecorateRotate>(csmanordecoraterotate, opCodeType, ResponseSCEmptyManorDecorateRotateCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSLike(CSLike cslike, Action<SCEmptyLike> ResponseSCEmptyLikeCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSLike> ();
            ProtoSendMethod.BusinessRequest<SCEmptyLike>(cslike, opCodeType, ResponseSCEmptyLikeCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSFriendCostume(CSFriendCostume csfriendcostume, Action<SCFriendCostume> ResponseSCFriendCostumeCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSFriendCostume> ();
            ProtoSendMethod.BusinessRequest<SCFriendCostume>(csfriendcostume, opCodeType, ResponseSCFriendCostumeCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSEmptyOnceWatering(CSEmptyOnceWatering csemptyoncewatering, Action<SCOnceWatering> ResponseSCOnceWateringCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSEmptyOnceWatering> ();
            ProtoSendMethod.BusinessRequest<SCOnceWatering>(csemptyoncewatering, opCodeType, ResponseSCOnceWateringCallBack, errorCallBack, isShowDefaultTip);
        }
		public void SendCSDerectUp(CSDerectUp csderectup, Action<SCEmptyDerectUp> ResponseSCEmptyDerectUpCallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <CSDerectUp> ();
            ProtoSendMethod.BusinessRequest<SCEmptyDerectUp>(csderectup, opCodeType, ResponseSCEmptyDerectUpCallBack, errorCallBack, isShowDefaultTip);
        }
		
    }
}




