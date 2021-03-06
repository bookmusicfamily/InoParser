﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace InoonLoRaParser.Upstream
{
    public class UpstreamApplicationPayload
    {

        public const Double accScale10bit = 1000.0 / 256.0; //3.91; //mg for 2G range. 10bit for BMA250e   
        public const Double accScale14bit = 1000.0 / 4096.0; //0.244; //mg for 2G range. 14bit BMA280 

        private const int aliveLengthVer1 = 14;
        private const int aliveLengthVer2 = 50;
        private const int aliveLengthVer3 = 50;
        private const int eventLengthVer1 = 6;
        private const int eventLengthVer2 = 10;
        private const int eventLengthVer_inclination = 16;

        //parse application payload 
        public string parseApplicationPayload(UpstreamCommon.UpPacketType pktType, string pktStr, int version)
        {
            string res = String.Empty;

            res = parseApplicationPayload(pktType, pktStr, version, UpstreamCommon.DeviceType.InoVibe_MGI100);

            return res; 
        }

        //parse application payload 
        public string parseApplicationPayload(UpstreamCommon.UpPacketType pktType, string pktStr, int version, UpstreamCommon.DeviceType devType)
        {
            string res = String.Empty;

            if (pktType == UpstreamCommon.UpPacketType.Alive)
                res = parseAlive(pktStr, version, devType);
            else if (pktType == UpstreamCommon.UpPacketType.Event)
                res = parseEvent(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.Error)
                res = parseError(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.Ack)
                res = parseAck(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.Notice)
                res = parseNotice(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.DataLog)
                res = parseDataLog(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.Report)
                res = parseReport(pktStr, version, devType);
            else if (pktType == UpstreamCommon.UpPacketType.AccelWaveform)
                res = parseAccelWaveform(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.Inclination)
                res = parseInclination(pktStr, version);
            else if (pktType == UpstreamCommon.UpPacketType.MachineRuntimeMeasurement)
                res = parseMachineRuntimeMeasurement(pktStr, version, devType);
            else if (pktType == UpstreamCommon.UpPacketType.MachineRuntimeReport)
                res = parseMachineRuntimeReport(pktStr, version, devType);
            else
                res = "Unknown packet\n";


            return res;
        }


        public string parseAlive(string payload, int version)
        {            
            return parseAlive(payload, version, UpstreamCommon.DeviceType.InoVibe_MGI100); 
        }


        public string parseAlive(string payload, int version, UpstreamCommon.DeviceType devType)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String subStr;
            int len = 2;
            int strLen = 0;
            Double accScaleDevType; 
            
            if (devType == UpstreamCommon.DeviceType.InoVibe_MGI100N) // BMA280 added 
                accScaleDevType = accScale14bit;
            else //No other device type yet 
                accScaleDevType = accScale10bit;


            sb.Append("Alive");
            sb.AppendLine();
            
            // Trim whitespace
            payload = payload.Trim();

            strLen = payload.Length;

            if ((1 == version) && (aliveLengthVer1 == strLen))
            {

                // X axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int xaxis = Convert.ToInt16(subStr, 16);
                Double xconv = accScaleDevType * (Double)xaxis;
                sb.AppendFormat("X axis: {0:N1} mg", xconv);
                sb.AppendLine();
                index += len;


                // Y axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int yaxis = Convert.ToInt16(subStr, 16);
                Double yconv = accScaleDevType * (Double)yaxis;
                sb.AppendFormat("Y axis: {0:N1} mg", yconv);
                sb.AppendLine();
                index += len;

                // Z axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int zaxis = Convert.ToInt16(subStr, 16);
                Double zconv = accScaleDevType * (Double)zaxis;
                sb.AppendFormat("Z axis: {0:N1} mg", zconv);
                sb.AppendLine();
                index += len;

                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }            
            else if ((2 == version) && (aliveLengthVer2 == strLen))// version 2 
            {
                // X axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int xaxis = Convert.ToInt16(subStr, 16);
                Double xconv = accScaleDevType * (Double)xaxis;
                sb.AppendFormat("X axis: {0:N1} mg", xconv);
                sb.AppendLine();
                index += len;


                // Y axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int yaxis = Convert.ToInt16(subStr, 16);
                Double yconv = accScaleDevType * (Double)yaxis;
                sb.AppendFormat("Y axis: {0:N1} mg", yconv);
                sb.AppendLine();
                index += len;

                // Z axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int zaxis = Convert.ToInt16(subStr, 16);
                Double zconv = accScaleDevType * (Double)zaxis;
                sb.AppendFormat("Z axis: {0:N1} mg", zconv);
                sb.AppendLine();
                index += len;


                // Alive Period  (min) 
                len = 4;
                subStr = payload.Substring(index, len);
                int alivePeriod = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Alive period: {0} min", alivePeriod);
                sb.AppendLine();
                index += len;

                
                // Acc Config -- Sensitivity level 
                len = 2;
                subStr = payload.Substring(index, len);
                int sensitivityLevel = Convert.ToInt16(subStr, 16);
                int sensitivityGrav = 1; 
                for (int i = 0; i < sensitivityLevel; i++)
                {
                    sensitivityGrav *= 2; 
                }
                sb.AppendFormat("Sensitivity: {0} G ", sensitivityGrav.ToString());
                sb.AppendLine();
                index += len;

                // Acc Config -- Sensitivity Threshlod 
                len = 4;
                subStr = payload.Substring(index, len);
                int sensitivityThesh = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Sensitivity Threshlod : {0} mg", sensitivityThesh.ToString());
                sb.AppendLine();
                index += len;

                // Acc Interrupt 
                
                // Acc Interrupt No 
                len = 2;
                subStr = payload.Substring(index, len);
                int accIntrNum = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Acc Interrupt No: {0} ", accIntrNum);
                sb.AppendLine();
                index += len;

                // Skip reserved field 
                len = 2;
                index += len;

                // Acc Interrupt :  Data field 
                len = 2;
                sb.Append("Acc Interrupt enabled axis : ");
                subStr = payload.Substring(index, len);

                //SByte accByte = Convert.ToSByte(subStr);
                Byte accByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                string bitFlag = Convert.ToString(accByte, 2).PadLeft(8, '0'); // 00010001

                var chars = bitFlag.ToCharArray();
                
                if (chars[5].Equals('1'))
                    sb.Append("X");

                if (chars[6].Equals('1'))
                    sb.Append("Y");

                if (chars[7].Equals('1'))
                    sb.Append("Z");

                sb.AppendLine();
                index += len;


                // Data log enable 
                len = 2;
                subStr = payload.Substring(index, len);
                int logEnable = Convert.ToInt16(subStr, 16);
                
                if (logEnable == 0)
                {
                    sb.Append("Acc Data Log Disabled. ");
                }
                else
                {
                    sb.Append("Acc Data Log Enabled. ");
                }
                                
                sb.AppendLine();
                index += len;

                // Data log Interval  
                len = 2;
                subStr = payload.Substring(index, len);
                int logInterval = Convert.ToInt16(subStr, 16);
                logInterval = 10 * logInterval; // predefined spec. 
                sb.AppendFormat("Acc Interval : {0} ms ", logInterval.ToString());

                sb.AppendLine();
                index += len;

                // Data log :  No of Biocks 
                len = 2;
                subStr = payload.Substring(index, len);
                int numBlocks = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Acc Blocks : {0} ", numBlocks.ToString());

                sb.AppendLine();
                index += len;

                // Setup message : installed or not 
                len = 2;
                subStr = payload.Substring(index, len);
                int installStatus = Convert.ToInt16(subStr, 16);

                if (installStatus == 0)
                {
                    sb.Append("Uninstalled ");
                }
                else
                {
                    sb.Append("Installed ");
                }

                sb.AppendLine();
                index += len;


                // FW Version 
                // Major 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwMajor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("FW version: {0} ", fwMajor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Minor 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwMinor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwMinor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Revision 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwRev = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwRev.ToString("00"));
                sb.AppendLine();
                index += len;


                // LoRa module FW Version (SoluM)
                // Major 
                len = 2;
                subStr = payload.Substring(index, len);
                fwMajor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("LoRa Module FW version: {0} ", fwMajor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Minor 
                len = 2;
                subStr = payload.Substring(index, len);
                fwMinor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwMinor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Revision 
                len = 2;
                subStr = payload.Substring(index, len);
                fwRev = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwRev.ToString("00"));
                sb.AppendLine();
                index += len;


                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);

                sb.AppendLine();
                index += len;
                

            }
            else if ((3 == version) && (aliveLengthVer3 == strLen))// version 2 
            {
                // X axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int xaxis = Convert.ToInt16(subStr, 16);
                Double xconv = accScale10bit * (Double)xaxis;
                sb.AppendFormat("X axis: {0:N1} mg", xconv);
                sb.AppendLine();
                index += len;


                // Y axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int yaxis = Convert.ToInt16(subStr, 16);
                Double yconv = accScaleDevType * (Double)yaxis;
                sb.AppendFormat("Y axis: {0:N1} mg", yconv);
                sb.AppendLine();
                index += len;

                // Z axis 
                len = 4;
                subStr = payload.Substring(index, len);
                int zaxis = Convert.ToInt16(subStr, 16);
                Double zconv = accScaleDevType * (Double)zaxis;
                sb.AppendFormat("Z axis: {0:N1} mg", zconv);
                sb.AppendLine();
                index += len;


                // Alive Period  (min) 
                len = 4;
                subStr = payload.Substring(index, len);
                int alivePeriod = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Alive period: {0} min", alivePeriod);
                sb.AppendLine();
                index += len;


                // Acc Config -- Sensitivity level 
                len = 2;
                subStr = payload.Substring(index, len);
                int sensitivityLevel = Convert.ToInt16(subStr, 16);
                int sensitivityGrav = 1;
                for (int i = 0; i < sensitivityLevel; i++)
                {
                    sensitivityGrav *= 2;
                }
                sb.AppendFormat("Sensitivity: {0} G ", sensitivityGrav.ToString());
                sb.AppendLine();
                index += len;

                // Acc Config -- Sensitivity Threshlod 
                len = 4;
                subStr = payload.Substring(index, len);
                int sensitivityThesh = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Sensitivity Threshlod : {0} mg", sensitivityThesh.ToString());
                sb.AppendLine();
                index += len;

                // Acc Interrupt 

                // Acc Interrupt No 
                len = 2;
                subStr = payload.Substring(index, len);
                int accIntrNum = Convert.ToInt16(subStr, 16);

                //sb.AppendFormat("Acc Interrupt No: {0} ", accIntrNum);

                if (1 == accIntrNum)
                    sb.Append("Acc Interrupt source: High-g interrupt");
                else if (2 == accIntrNum)
                    sb.Append("Acc Interrupt source: Slope detection interrupt");
                else
                    sb.AppendFormat("Acc Interrupt source: Unknown {0} ", accIntrNum);


                sb.AppendLine();
                index += len;

                // Skip reserved field 
                //len = 2;
                //index += len;

                // jychoi 20191014 BMA sleep duration 
                len = 2;
                subStr = payload.Substring(index, len);
                int sleepDur = Convert.ToInt16(subStr, 16);
                sb.Append("BMA sleep duration: ");

                if (0 == sleepDur)
                    sb.Append("10 ms");
                else if (6 > sleepDur)
                    sb.Append("0.5 ms");
                else if (6 == sleepDur)
                    sb.Append("1 ms");
                else if (7 == sleepDur)
                    sb.Append("2 ms");
                else if (8 == sleepDur)
                    sb.Append("4 ms");
                else if (9 == sleepDur)
                    sb.Append("6 ms");
                else if (10 == sleepDur)
                    sb.Append("10 ms");
                else if (11 == sleepDur)
                    sb.Append("25 ms");
                else if (12 == sleepDur)
                    sb.Append("50 ms");
                else if (13 == sleepDur)
                    sb.Append("100 ms");
                else if (14 == sleepDur)
                    sb.Append("500 ms");
                else if (15 == sleepDur)
                    sb.Append("1 sec");
                else
                    sb.AppendFormat("Unknown {0} ", sleepDur);


                sb.AppendLine();
                index += len;


                // Acc Interrupt :  Data field 
                len = 2;
                sb.Append("High-g Interrupt enabled axis : ");
                subStr = payload.Substring(index, len);

                //SByte accByte = Convert.ToSByte(subStr);
                Byte accByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                string bitFlag = Convert.ToString(accByte, 2).PadLeft(8, '0'); // 00010001

                var chars = bitFlag.ToCharArray();

                if (chars[5].Equals('1'))
                    sb.Append("X");

                if (chars[6].Equals('1'))
                    sb.Append("Y");

                if (chars[7].Equals('1'))
                    sb.Append("Z");

                sb.AppendLine();
                index += len;


                // Data log enable 
                len = 2;
                subStr = payload.Substring(index, len);
                int logEnable = Convert.ToInt16(subStr, 16);

                if (logEnable == 0)
                {
                    sb.Append("Acc Data Log Disabled. ");
                }
                else
                {
                    sb.Append("Acc Data Log Enabled. ");
                }

                sb.AppendLine();
                index += len;

                // Data log Interval  
                len = 2;
                subStr = payload.Substring(index, len);
                int logInterval = Convert.ToInt16(subStr, 16);
                logInterval = 10 * logInterval; // predefined spec. 
                sb.AppendFormat("Acc Interval : {0} ms ", logInterval.ToString());

                sb.AppendLine();
                index += len;

                // Data log :  No of Biocks 
                len = 2;
                subStr = payload.Substring(index, len);
                int numBlocks = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("Acc Blocks : {0} ", numBlocks.ToString());

                sb.AppendLine();
                index += len;

                // Setup message : installed or not 
                len = 2;
                subStr = payload.Substring(index, len);
                int installStatus = Convert.ToInt16(subStr, 16);

                if (installStatus == 0)
                {
                    sb.Append("Uninstall state");
                }
                else if (installStatus == 1)
                {
                    sb.Append("Install state");
                }
                else if (installStatus == 2)
                {
                    sb.Append("PrepareInstall state");
                }
                else
                {
                    sb.Append("Unknown setup state");
                }

                sb.AppendLine();
                index += len;



                // FW Version 
                // Major 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwMajor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("FW version: {0} ", fwMajor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Minor 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwMinor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwMinor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Revision 
                len = 2;
                subStr = payload.Substring(index, len);
                int fwRev = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwRev.ToString("00"));
                sb.AppendLine();
                index += len;


                // LoRa module FW Version (SoluM)
                // Major 
                len = 2;
                subStr = payload.Substring(index, len);
                fwMajor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat("LoRa Module FW version: {0} ", fwMajor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Minor 
                len = 2;
                subStr = payload.Substring(index, len);
                fwMinor = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwMinor.ToString("00"));
                //sb.AppendLine();
                index += len;

                // Revision 
                len = 2;
                subStr = payload.Substring(index, len);
                fwRev = Convert.ToInt16(subStr, 16);
                sb.AppendFormat(" {0} ", fwRev.ToString("00"));
                sb.AppendLine();
                index += len;
                
            }
            else
            {
                sb.Append("Invalid Alive Message Format");
                sb.AppendLine();
            }

            return sb.ToString(); 
        }

        public static int convertSignedByteToInt(int rssi)
        {
            int result = 0; 
            if (rssi > 127)
                result = (256 - rssi) * (-1); 
            else
                result = rssi;

            return result;

        }

        public string parseEvent(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String eventType; 
            String subStr;
            int len = 2;
            
            sb.Append("Event");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Check packet validity 
            int strLen = payload.Length;

            // Version 1 
            if ((1 == version) && (eventLengthVer1 == strLen))
            {
                // Event type 
                len = 2;
                eventType = payload.Substring(index, len);
                string eventString;
                switch (eventType)
                {
                    case "01":
                        eventString = "High-G";
                        break;
                    case "02":
                        eventString = "Tap";
                        break;
                    case "03":
                        eventString = "Double tap";
                        break;
                    case "04":
                        eventString = "Orient";
                        break;
                    case "05":
                        eventString = "Flip";
                        break;
                    case "06":
                        eventString = "Flat";
                        break;
                    case "07":
                        eventString = "Low-G";
                        break;
                    case "08":
                        eventString = "Collapse"; //jychoi 
                        break;
                    default:
                        eventString = "Unknown event";
                        break;
                }


                sb.Append(eventString);
                sb.AppendLine();
                index += len;

                // X Y Z bit 
                len = 2;
                subStr = payload.Substring(index, len);

                //SByte accByte = Convert.ToSByte(subStr);
                Byte accByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);


                string bitFlag = Convert.ToString(accByte, 2).PadLeft(8, '0'); // 00010001

                var chars = bitFlag.ToCharArray();

                if (chars[4].Equals('1'))
                    sb.Append("-");
                else
                    sb.Append("+");

                if (chars[5].Equals('1'))
                    sb.Append("Z");

                if (chars[6].Equals('1'))
                    sb.Append("Y");

                if (chars[7].Equals('1'))
                    sb.Append("X");

                sb.AppendLine();
                index += len;

            }
            // Version 2 
            else if ((eventLengthVer2 == strLen) || (eventLengthVer_inclination == strLen))
            {
                // Event type 
                len = 2;
                eventType = payload.Substring(index, len);
                string eventString;
                switch (eventType)
                {
                    case "01":
                        eventString = "High-G";
                        break;
                    case "02":
                        eventString = "Slope detection";
                        break;
                    default:
                        eventString = "Unknown event";
                        break;
                }


                sb.Append(eventString);
                sb.AppendLine();
                index += len;

                if (eventString.Equals("High-G") || eventString.Equals("Slope detection"))
                {
                    // X axis 
                    len = 2;
                    subStr = payload.Substring(index, len);
                    int xnum = Convert.ToInt16(subStr, 16);
                    sb.AppendFormat("Interrupt counts X : {0} ", xnum.ToString());
                    sb.AppendLine();
                    index += len;


                    // Y axis 
                    len = 2;
                    subStr = payload.Substring(index, len);
                    int ynum = Convert.ToInt16(subStr, 16);
                    sb.AppendFormat("Interrupt counts Y: {0} ", ynum.ToString());
                    sb.AppendLine();
                    index += len;

                    // Z axis 
                    len = 2;
                    subStr = payload.Substring(index, len);
                    int znum = Convert.ToInt16(subStr, 16);
                    sb.AppendFormat("Interrupt counts Z: {0} ", znum.ToString());
                    sb.AppendLine();
                    index += len;

                }
                else
                {

                    sb.AppendFormat("Unknown event type");
                    sb.AppendLine();
                }
                
            }
            else
            {
                sb.Append("Invalid Event Message Format");
                sb.AppendLine();
            }

            if (version < 3)
            {
                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }

            return sb.ToString();
        }
        
        public string parseError(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String eventType;
            String subStr;
            int len = 2;

            sb.Append("Error");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Error type 
            len = 2;
            eventType = payload.Substring(index, len);
            string eventString;
            switch (eventType)
            {
                case "00":
                    eventString = "LoraErrorNone";
                    break;
                case "01":
                    eventString = "LoraErrorOverReTx";
                    break;
                case "02":
                    eventString = "LoraErrorJoinFail";
                    break;
                case "03":
                    eventString = "LoraErrorRx";
                    break;
                case "04":
                    eventString = "LoraErrorNok";
                    break;
                case "05":
                    eventString = "LoraErrorOverReTxInfinite";
                    break;
                case "06":
                    eventString = "LoraErrorSkipHighG";
                    break;
                case "07":
                    eventString = "LoraErrorFlashStoreFail";
                    break;
                case "08":
                    eventString = "LoraErrorFlashLoadFail";
                    break;
                case "09":
                    eventString = "LoraErrorFlashClearFail";
                    break;
                case "0a":
                case "0A":
                    eventString = "LoraErrorFlashUpdateFail";
                    break;
                case "0b":
                case "0B":
                    eventString = "LoraErrorPowerFailure";
                    break;
                case "0c":
                case "0C":
                    eventString = "LoraErrorBeaconStartFail";
                    break;
                case "0d":
                case "0D":
                    eventString = "LoraErrorCheckFWFail";
                    break;
                case "0e":
                case "0E":
                    eventString = "LoraErrorAccelerometerFail";
                    break;
                case "0f":
                case "0F":
                    eventString = "LoraErrorUnexpectedSystemMode";
                    break;
                default:
                    eventString = "LoraErrorUnknown";
                    break;
            }


            sb.Append(eventString);
            sb.AppendLine();
            index += len;

            if (version < 3)
            {

                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }

            return sb.ToString();
        }
                
        public string parseAck(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String eventType;
            String subStr;
            int len = 2;

            sb.Append("Ack");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Ack type 
            len = 2;
            eventType = payload.Substring(index, len);
            string eventString;
            switch (eventType)
            {
                case "00":
                    eventString = "ExtDevMgmt";
                    break;
                case "80":
                    eventString = "DevReset";
                    break;
                case "81":
                    eventString = "RepPerChange";
                    break;
                case "82":
                    eventString = "RepImmediate (Class C)";
                    break;
                default:
                    eventString = "Unknown Ack";
                    break;
            }


            sb.Append(eventString);
            sb.AppendLine();
            index += len;

            if (version < 3)
            {

                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }

            return sb.ToString();
        }

        public string parseNotice(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String noticeType;
            String subStr;
            int len = 2;

            sb.Append("Notice");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Notice type 
            len = 2;
            noticeType = payload.Substring(index, len); // do not touch noticeType. It will be used to parse payload. 
            string notiString;
            switch (noticeType) 
            {
                case "01":
                    notiString = "Power Up";
                    break;
                case "02":
                    notiString = "Power Off";
                    break;
                case "03":
                    notiString = "Reset";
                    break;
                case "04":
                    notiString = "Setup (BLE)";
                    break;
                case "05":
                    notiString = "Test Result";
                    break;
                case "06":
                    notiString = "Rejection Count";
                    break;
                case "07":
                    notiString = "Application Mode Config";
                    break;
                case "FF":
                    notiString = "String. Not yet supported.";
                    break;
                default:
                    notiString = "Unknown notice";
                    break;
            }


            sb.Append(notiString);
            sb.AppendLine();
            index += len;


            // Notice Length  
            len = 2;
            subStr = payload.Substring(index, len);
            int noticeLength = Convert.ToInt16(subStr, 16);
            
            sb.AppendFormat("Length {0}", noticeLength);

            sb.AppendLine();
            index += len;


            // Notice Payload  
            len = (noticeLength * 2);//1Byte is 2 digit string. 
            subStr = payload.Substring(index, len);
            //subStr = payload.Substring(index, (noticeLength * 2)); 

            switch (noticeType)
            {
                case "01":
                    notiString = parseNoticePowerUp(subStr);
                    break;
                case "02":
                    notiString = parseNoticePowerOff(subStr);
                    break;
                case "03":
                    notiString = parseNoticeReset(subStr);
                    break;
                case "04":
                    notiString = parseNoticeSetup(subStr); // Uninstall / install 
                    break;
                case "05":
                    notiString = parseNoticeTestResult(subStr); 
                    break;
                case "06":
                    notiString = parseNoticeShockRejectionCount(subStr);
                    break;
                case "07":
                    notiString = parseNoticeApplicationModeConfig(subStr);
                    break;
                case "FF":
                    notiString = "String payload. Not yet supported.";
                    break;
                default:
                    notiString = "Unknown notice";
                    break;
            }
            sb.Append(notiString);
            sb.AppendLine();
            index += len;

            if (version < 3)
            {
                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }

            return sb.ToString();
        }

        public string parseReport(string payload, int version)
        {
            return parseReport(payload, version, UpstreamCommon.DeviceType.InoVibe_MGI100);
        }

        public string parseReport(string payload, int version, UpstreamCommon.DeviceType devType)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            //String noticeType;
            String subStr;
            int len = 2;
            double acc_adapt_num; // For BMA250e

            if (devType == UpstreamCommon.DeviceType.InoVibe_MGI100N)
                acc_adapt_num = accScale14bit; 
            else
                acc_adapt_num = accScale10bit;

            sb.Append("Report");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Report Header 
            len = 2;
            subStr = payload.Substring(index, len);

            //SByte accByte = Convert.ToSByte(subStr);
            Byte accByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            // X Y Z bit 
            string bitFlag = Convert.ToString(accByte, 2).PadLeft(8, '0'); // 00010001

            var chars = bitFlag.ToCharArray();

            

            if (chars[0].Equals('1'))
                sb.Append("Calc method : Float");
            else
                sb.Append("Calc method : Int");

            sb.AppendLine();

            if (chars[1].Equals('1'))
                sb.Append("Overflow Yes");
            else
                sb.Append("Overflow No");

            sb.AppendLine();

            if ((chars[2].Equals('0')) && (chars[3].Equals('0')))
                sb.Append("X or CVA");

            if ((chars[2].Equals('0')) && (chars[3].Equals('1')))
                sb.Append("Y");

            if ((chars[2].Equals('1')) && (chars[3].Equals('0')))
                sb.Append("Z");

            sb.AppendLine();

            
            byte y = 0x0F;
            int scaleFactor = 1;

            scaleFactor = (int)(accByte & y);

            sb.AppendFormat("Scale Factor : {0}", scaleFactor);
            sb.AppendLine();


            index += len;

            //////////////////////
            // Average 
            len = 4;
            subStr = payload.Substring(index, len);
            int average = Convert.ToInt16(subStr, 16);
            double averageDouble = Convert.ToDouble(average);
            for (int i = 0; i < scaleFactor; i++)
                averageDouble = averageDouble / 2;

            averageDouble = averageDouble * acc_adapt_num;

            sb.AppendFormat("Average {0} (mg)", Math.Round(averageDouble, 2));
            sb.AppendLine();
            
            index += len;

            //////////////////////
            // STDEV 
            len = 4;
            subStr = payload.Substring(index, len);
            int stdev = Convert.ToInt16(subStr, 16);
            double stdevDouble = Convert.ToDouble(stdev);
            for (int i = 0; i < scaleFactor; i++)
                stdevDouble = stdevDouble / 2;

            stdevDouble = stdevDouble * acc_adapt_num;

            sb.AppendFormat("STDEV {0} (mg)", Math.Round(stdevDouble, 2));
            sb.AppendLine();

            index += len;


            //////////////////////
            // MIN 
            len = 4;
            subStr = payload.Substring(index, len);
            int min = Convert.ToInt16(subStr, 16);
            double minDouble = Convert.ToDouble(min);
            for (int i = 0; i < scaleFactor; i++)
                minDouble = minDouble / 2;

            minDouble = minDouble * acc_adapt_num;

            sb.AppendFormat("MIN {0} (mg)", Math.Round(minDouble, 2));
            sb.AppendLine();

            index += len;


            //////////////////////
            // MAX 
            len = 4;
            subStr = payload.Substring(index, len);
            int max = Convert.ToInt16(subStr, 16);
            double maxDouble = Convert.ToDouble(max);
            for (int i = 0; i < scaleFactor; i++)
                maxDouble = maxDouble / 2;

            maxDouble = maxDouble * acc_adapt_num;

            sb.AppendFormat("Max {0} (mg)", Math.Round(maxDouble, 2));
            //sb.AppendLine();

            index += len;
                        

            if (version < 3)
            {
                // RSSI
                len = 2;
                subStr = payload.Substring(index, len);
                int rssi = Convert.ToInt16(subStr, 16);
                rssi = convertSignedByteToInt(rssi);
                sb.AppendLine();
                sb.AppendFormat("RSSI: {0} ", rssi);
                sb.AppendLine();
                index += len;

            }

            return sb.ToString();
        }



        private String parsePowerOffErrorLog(String subStr)
        {
            StringBuilder sb = new StringBuilder();
            try
            {

                switch (subStr)
                {
                    case "00":
                        sb.Append("No Power-off record. ");
                        break;
                    case "01":
                        sb.Append("Alive 주기 초과 (전원 리셋) : 내부 에러 발생으로 인해 정해진 Alive 주기에 전송을 못해서 +5분 초과하는 경우 리셋됨.");
                        break;
                    case "02":
                        sb.Append("LoRa 전송 실패 (전원 꺼짐) : 전송(재전송횟수 8회)이 10회 실패하면 전원 꺼짐.");
                        break;
                    case "04":
                        sb.Append("LoRa Join 실패 (전원 꺼짐) : Join Request 가 연속 24회 실패하면 전원 꺼짐.");
                        break;
                    case "05":
                        sb.Append("PowerOffWatchdogTimeout (리셋): Watchdog Timeout 통한 전원 리셋");
                        break;
                    case "06":
                        sb.Append("PowerOffNoAckOfEndPacket (전원 꺼짐): End packet 전송 후 일정시간 동안 Ack 못 받은 경우 전원 꺼짐");
                        break;
                    case "07":
                        sb.Append("PowerOffLoRaRxCmdPowerOff (전원 꺼짐): LoRa downstream 통해 PowerOff 명령(0x01) 수신하여 전원 꺼짐");
                        break;
                    case "08":
                        sb.Append("PowerOffBLERxCmdPowerOff (전원 꺼짐): BLE 통해 PowerOff 명령(0x92) 수신하여 전원 꺼짐");
                        break;
                    case "09":
                        sb.Append("PowerOffLowBattery (전원 꺼짐): Low battery 상태가 되어 전원 꺼짐");
                        break;
                    case "0A":
                    case "0a":
                        sb.Append("PowerOffUninstalled (전원 꺼짐): 설치되지 않은 디바이스의 전송 횟수 초과로 전원 꺼짐");
                        break;
                    case "0B":
                    case "0b":
                        sb.Append("PowerOffInitConfigOff (전원 꺼짐): INIT_CONFIG_OFF 통해 Configuration 정보를 reset 한 후 전원 꺼짐");
                        break;
                    case "0C":
                    case "0c":
                        sb.Append("PowerOffUninstallCommand (전원 꺼짐): Uninstall Command 수신하여 전원 꺼짐 (BLE or LoRa downstream)");
                        break;
                    case "0D":
                    case "0d":
                        sb.Append("PowerOffUninstallByReserved (전원 꺼짐): Uninstall Command 수신한 이후, 가속도 데이터 전송 완료 후 전원 꺼짐 (BLE or LoRa downstream)");
                        break;
                    case "0E":
                    case "0e":
                        sb.Append("PowerOffDeviceUpsideDown (전원 꺼짐): 자석으로 전원 켜진 후 15초 이내에 뒤집었을 때 전원 꺼짐");
                        break;
                    case "0F":
                    case "0f":
                        sb.Append("PowerOffUninstallTimeout (전원 꺼짐): Uninstall 상테에서 10분 경과하면 자동으로 전원이 꺼진다.");
                        break;
                    case "81":
                        sb.Append("PowerOffResetFactorySetting (전원 리셋): Factory Setting 명령어 통해 전원 리셋");
                        break;
                    case "82":
                        sb.Append("PowerOffResetDeviceUpsideDown (전원 리셋): 15초 이하일 때 포지션이 바뀌면 전원 리셋");
                        break;
                    case "83":
                        sb.Append("PowerOffResetWatchdogConfig (전원 리셋): Watchdog configuration을 바꾸었을 때 전원 리셋");
                        break;
                    case "84":
                        sb.Append("PowerOffResetLoRaRxCmdReset (전원 리셋): LoRa downstream 통해 Reset 명령(0x02) 수신하여 전원 리셋");
                        break;
                    case "85":
                        sb.Append("PowerOffResetBLERxCmdReset (전원 리셋): BLE 통해 Reset 명령(0x02) 수신하여 전원 리셋");
                        break;
                    case "86":
                        sb.Append("PowerOffResetFactoryReset (전원 리셋): Factory Reset 수신하여 전원 리셋");
                        break;
                    case "87":
                        sb.Append("PowerOffResetSKTReset (전원 리셋): SKT_DEV_RESET 수신하여 전원 리셋");
                        break;
                    case "88":
                        sb.Append("PowerOffResetSoluMFWUpgrade (전원 리셋): SoluM FW upgrade 후 전원 리셋");
                        break;
                    case "89":
                        sb.Append("PowerOffResetInitConfigReset (전원 리셋): BLE로 Init_config_reset 명령 수신하여 전원 리셋 ");
                        break;
                    case "8a":
                    case "8A":
                        sb.Append("PowerOffResetPowerUpNoticeFail (전원 리셋): PowerUp Notice 전송 실패하여 전원 리셋  ");
                        break;
                    case "8b":
                    case "8B":
                        sb.Append("PowerOffResetPeriodicParamChanged (전원 리셋): Periodic report parameters are changed");
                        break;
                    case "8c":
                    case "8C":
                        sb.Append("PowerOffResetInclinationParamChanged (전원 리셋): Inclination monitoring parameters are changed");
                        break;
                    case "8d":
                    case "8D":
                        sb.Append("PowerOffResetMachineRuntimeConfigChanged (전원 리셋): Machine runtime configuration values are changed");
                        break;
                    case "8e":
                    case "8E":
                        sb.Append("PowerOffResetLoRaRetransmissionFail (전원 리셋): LoRa Transmission fail multiple times in AS923");
                        break;
                    case "8f":
                    case "8F":
                        sb.Append("PowerOffResetInfResetTest (전원 리셋): 무한 리셋 테스트");
                        break;
                    case "90":
                        sb.Append("PowerOffResetAccWaveformTxTimeout (전원 리셋): Waveform 전송 중 정해진 시간 안에 종료되지 않음");
                        break;
                    case "91":
                        sb.Append("PowerOffResetLoRaJoinFail (전원 리셋): Install 상태에서 LoRa Join 실패 시 Power off 대신 Reset");
                        break;
                    case "92":
                        sb.Append("PowerOffResetATSENDNOK (전원 리셋): AT+SEND NOK가 연속 8회 발생 시 Device reset");
                        break;
                    case "93":
                        sb.Append("PowerOffResetLoRaModuleAbnormalReset (전원 리셋): nRF 동작 중인데 LoRa 모듈만 Reset 된 경우를 가정");
                        break;
                    case "94":
                        sb.Append("PowerOffResetAliveFailed (전원 리셋): Alive msg transmission is failed for 2 times");
                        break;
                    case "95":
                        sb.Append("PowerOffResetChangedToImpactMode (전원 리셋): Impact Monitoring mode 로 바뀌어 reset");
                        break;
                    case "96":
                        sb.Append("PowerOffResetChangedToExcvInclMode (전원 리셋): Excavation_Inclination Monitoring mode 로 바뀌어 reset");
                        break;
                    case "97":
                        sb.Append("PowerOffResetChangedToMrtmMode (전원 리셋): Machine Runtime Monitoring mode 로 바뀌어 reset");
                        break;
                    case "98":
                        sb.Append("PowerOffResetBootCompleteRemained (전원 리셋): BootComplete state에 16초이상 머물러 있을 경우 리셋");
                        break;

                    default:
                        sb.Append("Unknown power off reason");
                        break;
                }


            }
            catch (FormatException)
            {
                sb.Append("Invalid error log in Notice.");
            }

            sb.AppendLine();

            return sb.ToString(); 

        }

        //  1st Byte : Reset reason 
        //  4th Byte : Error log 
        public string parseNoticePowerUp(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();

            // power up notice  

            /////////////////////////////////
            // Parse reset reason 
            sb.Append("Reset Reason: ");

            len = 2;
            subStr = payload.Substring(index, len);
            try
            {
                //SByte powerUpByte = Convert.ToSByte(subStr);
                Byte powerUpByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);


                string bitFlag = Convert.ToString(powerUpByte, 2).PadLeft(8, '0'); // 00010001

                var chars = bitFlag.ToCharArray();

                if (chars[1].Equals('1'))
                    sb.Append("Reset due to wake up from system OFF mode when wakeup is triggered from entering into debug interface mode. "); // Not used 

                if (chars[2].Equals('1'))
                    sb.Append("Reset due to wake up from system OFF mode when wakeup is	triggered from ANADETECT signal from LPCOMP. "); // Not used 

                if (chars[3].Equals('1'))
                    sb.Append("Reset due to wake up from system OFF mode when wakeup is	triggered from DETECT signal from GPIO. "); // Not used 

                if (chars[4].Equals('1'))
                    sb.Append("CPU lock-up Reset. ");

                if (chars[5].Equals('1'))
                    sb.Append("SYSRESETREQ Reset. ");

                if (chars[6].Equals('1'))
                    sb.Append("Watchdog Reset. ");

                if (chars[7].Equals('1'))
                    sb.Append("Pin-reset. ");

                if (subStr.Equals("00"))
                    sb.Append("Power-on-reset or a brown out reset. ");

            }
            catch (FormatException)
            {
                sb.Append("Invalid reset reason in Notice.");
            }


            sb.AppendLine();
            index += len;

            /////////////////////////////////
            // Turn on count 
            len = 2;
            subStr = payload.Substring(index, len);

            int turnOnCnt = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Turn-on count: {0}", turnOnCnt.ToString());
            sb.AppendLine();

            index += len;

            ////////////////////////////////////////
            // skip reserved field 

            //index += len; //skip 2nd byte 
            index += len; //skip 3rd byte 

            /////////////////////////////////
            // Parse Error log 
            sb.Append("Power Off Error Log: ");

            subStr = payload.Substring(index, len);

            String errStr = parsePowerOffErrorLog(subStr);
            sb.Append(errStr); 

            index += len;

            return sb.ToString();
        }


        public string parseNoticePowerOff(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();

            // power off notice  

            len = 2;
            subStr = payload.Substring(index, len);

            /*
            string payloadStr;

            switch (subStr)
            {
                case "01":
                    payloadStr = "LoRa Downstream - Power off command (0x1)";
                    break;
                case "02":
                    payloadStr = "BLE command (0x92)";
                    break;
                case "03":
                    payloadStr = "설치되지 않은 디바이스의 전송 횟수 초과";
                    break;
                case "04":
                    payloadStr = "배터리 부족";
                    break;
                case "05":
                    payloadStr = "에러";
                    break;
                default:
                    payloadStr = "Unknown power off notice ";
                    break;
            }

            sb.Append(payloadStr); 
            sb.AppendLine();
            */

            String errStr = parsePowerOffErrorLog(subStr);
            sb.Append(errStr);
            
            index += len;

            return sb.ToString();
        }


        public string parseNoticeReset(string payload)
        {
            
            StringBuilder sb = new StringBuilder();            

            return sb.ToString();
        }


        public string parseNoticeSetup(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();

            // setup notice 

            len = 2;
            subStr = payload.Substring(index, len);

            string payloadStr;
            switch (subStr)
            {
                case "00":
                    payloadStr = "Uninstall";
                    break;
                case "01":
                    payloadStr = "Install";
                    break;
                case "02":
                    payloadStr = "PrepareInstall";
                    break;
                default:
                    payloadStr = "Unknown notice setup message";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;

            return sb.ToString();
        }


        public string parseNoticeTestResult(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();

            // parse notice test result  
            // Enable flag 
            len = 2;
            subStr = payload.Substring(index, len);

            string payloadStr;
            switch (subStr)
            {
                case "00":
                    payloadStr = "Disable";
                    break;
                case "01":
                    payloadStr = "Enable";
                    break;
                default:
                    payloadStr = "Unknown notice enable field";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Test command 
            len = 2;
            subStr = payload.Substring(index, len);

            switch (subStr)
            {
                case "00":
                    payloadStr = "None";
                    break;
                case "01":
                    payloadStr = "Waveform test";
                    break;
                case "02":
                    payloadStr = "Alive test";
                    break;
                case "03":
                    payloadStr = "Reset test";
                    break;
                case "04":
                    payloadStr = "Watchdog test";
                    break;
                case "05":
                    payloadStr = "Inclination test";
                    break;
                case "06":
                    payloadStr = "Runtime test";
                    break;
                default:
                    payloadStr = "Unknown Test command field";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Test Count Limit
            len = 2;
            subStr = payload.Substring(index, len);

            int testCountLimit = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Limit: {0}", testCountLimit.ToString());

            sb.AppendLine();
            index += len;

            

            // Move to next flag             
            len = 2;
            subStr = payload.Substring(index, len);

            switch (subStr)
            {
                case "00":
                    payloadStr = "Move to next False. Unit test";
                    break;
                case "01":
                    payloadStr = "Move to next TRUE. Test all.";
                    break;
                default:
                    payloadStr = "Unknown MoveToNext field";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // CurrentCount            
            len = 2;
            subStr = payload.Substring(index, len);
            
            int currentCount = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Current Count: {0}", currentCount.ToString());

            sb.AppendLine();
            index += len;


            // Test running flag             
            len = 2;
            subStr = payload.Substring(index, len);

            switch (subStr)
            {
                case "00":
                    payloadStr = "TestManager is not running";
                    break;
                case "01":
                    payloadStr = "TestManager is running";
                    break;
                default:
                    payloadStr = "Unknown TestManager Running field";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Reserved           
            len = 4;
            subStr = payload.Substring(index, len);

            sb.AppendFormat("Reserved: {0}", subStr);

            sb.AppendLine();
            index += len;

            return sb.ToString();
        }

        public string parseNoticeShockRejectionCount(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();
            

            // Shock Rejection Count (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int srCount = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Shock Rejection Count: {0}", srCount.ToString());

            sb.AppendLine();
            index += len;



            // Accumulation Period (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int accPeriod = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Accumulation Period: {0} H", accPeriod.ToString());

            sb.AppendLine();
            index += len;



            // Shock Rejection Threshold (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int srthresh = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Shock Rejection Threshold (RMS): {0} mg", srthresh.ToString());

            sb.AppendLine();
            index += len;
            
            
            return sb.ToString();
        }

        // If inclination check period exceeds INCL_PERIOD_HOUR_BASE minutes, you should follow the rule below 
        // INCL_PERIOD_HOUR_BASE = 60 minute 
        // INCL_PERIOD_HOUR_BASE + 1 = 61 = 1H
        // INCL_PERIOD_HOUR_BASE + 2 = 62 = 2H
        // INCL_PERIOD_HOUR_BASE + 3 = 63 = 3H
        private int getInclinationScaledPeriod(int incl_period)
        {
            int incl_period_conv = 0;
            int incl_hour_base = 60;
            int incl_oneday_base = 60 + 24;


            if (incl_period <= incl_hour_base)
            {
                incl_period_conv = incl_period;
            }
            else if ((incl_hour_base < incl_period) && (incl_period <= incl_oneday_base))
            {
                incl_period_conv = (incl_period - incl_hour_base) * 60;
            }
            else // ((INCL_PERIOD_HOUR_ONE_DAY < incl_period) && (incl_period < INCL_PERIOD_HOUR_INFINITY))
            {
                // MAX 24Hour 
                incl_period_conv = (incl_oneday_base - incl_hour_base) * 60;  //1440
            }

            return incl_period_conv;

        }

        public string parseNoticeApplicationModeConfig(string payload)
        {
            String subStr = String.Empty;
            int index = 0;
            int len = 2;
            StringBuilder sb = new StringBuilder();


            // Application Mode (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int appModeNum = Convert.ToUInt16(subStr, 16);

            sb.Append("Application Mode: "); 

            if (appModeNum == 0)
                sb.Append("Excavation & Inclination");
            else if (appModeNum == 1)
                sb.Append("Machine Runtime Monitoring");
            else if (appModeNum == 2)
                sb.Append("Impact Monitoring");
            else
                sb.Append("Unknown");

            sb.AppendLine();
            index += len;


            // BMA g range (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int gRangeNum = Convert.ToUInt16(subStr, 16);

            sb.Append("BMA g range: ");

            if (gRangeNum == 1)
                sb.Append("2g");
            else if (gRangeNum == 2)
                sb.Append("4g");
            else if (gRangeNum == 3)
                sb.Append("8g");
            else if (gRangeNum == 4)
                sb.Append("16g");
            else
                sb.Append("Unknown ");

            sb.AppendLine();
            index += len;


            // BMA High g threshold (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int highThresh = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("BMA High g Threshold: {0} mg", highThresh);
            
            sb.AppendLine();
            index += len;

            // nRF Shock Rejection Sub-interval  (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int srSubInterval = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Excavation Rejection Sub-interval: {0}", srSubInterval);

            sb.AppendLine();
            index += len;

            // nRF Rejection Threshold (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int srThresh = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Excavation Rejection Threshold: {0} mg", srThresh);

            sb.AppendLine();
            index += len;

            // nRF Impact threshold (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int impactThresh = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("nRF Impact Threshold: {0} mg", impactThresh);

            sb.AppendLine();
            index += len;


            // nRF inclination polling period  (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            sb.Append("nRF Inclination Polling Period: ");

            int inclPollingPeriod = Convert.ToUInt16(subStr, 16);
            
            if (inclPollingPeriod <= 60)
            {
                sb.AppendFormat("{0} minute(s)", inclPollingPeriod);
            }
            else if ((60 < inclPollingPeriod) && (inclPollingPeriod <= 60 + 24))
            {
                sb.AppendFormat("{0} hour(s)", (inclPollingPeriod - 60) );
            }
            else 
            {
                sb.AppendFormat("{0} hour(s)", (inclPollingPeriod - (60 + 24)));
            }
            
            sb.AppendLine();
            index += len;


            // Inclination Tx Skip Count  (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int txSkipCount = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Inclination Tx Skip Count: {0}", txSkipCount);

            sb.AppendLine();
            index += len;


            // Inclination accelerometer value of base angle X (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int baseAccX = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Inclination Base Acc. X: {0}", baseAccX);

            sb.AppendLine();
            index += len;

            // Inclination accelerometer value of base angle Y (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int baseAccY = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Inclination Base Acc. Y: {0}", baseAccY);

            sb.AppendLine();
            index += len;


            // Inclination accelerometer value of base angle X (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int baseAccZ = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Inclination Base Acc. Z: {0}", baseAccZ);

            sb.AppendLine();
            index += len;



            // Reserved  (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int inclReserved = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Inclination Reserved: {0}", inclReserved);

            sb.AppendLine();
            index += len;


            // Machine Runtime Monitoring State  (1B) 
            len = 2;
            subStr = payload.Substring(index, len);

            int mrtmState = Convert.ToUInt16(subStr, 16);
            
            sb.Append("MRTM state: ");

            if (mrtmState == 0)
                sb.Append("Commissioning state");
            else if (mrtmState == 1)
                sb.Append("Inactive state");
            else if (mrtmState == 2)
                sb.Append("Active state");
            else
                sb.Append("Unknown state");

            sb.AppendLine();
            index += len;


            // MRTM operation threshold  (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int mrtmOperThresh = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("MRTM Operation Threshold: {0} mg", mrtmOperThresh);

            sb.AppendLine();
            index += len;


            // MRTM shock threshold  (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int mrtmShockThresh = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("MRTM Shock Threshold: {0} mg", mrtmShockThresh);

            sb.AppendLine();
            index += len;

            // MRTM reserved  (2B) 
            len = 4;
            subStr = payload.Substring(index, len);

            int mrtmReserved = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("MRTM Reserved: {0}", mrtmReserved);

            sb.AppendLine();
            index += len;


            // Common reserved  (2B) 
            len = 8;
            subStr = payload.Substring(index, len);

            int generalReserved = Convert.ToUInt16(subStr, 16);

            sb.AppendFormat("Reserved: {0}", generalReserved);

            sb.AppendLine();
            index += len;


            return sb.ToString();
        }


        public string parseDataLog(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String dataLogPayload;
            String subStr;
            int len = 2;

            sb.Append("Data Log");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            // Data index 
            len = 2;
            dataLogPayload = payload.Substring(index, len); // 
            sb.AppendFormat("Data index: 0x{0}", dataLogPayload);                         
            sb.AppendLine();
            index += len;


            // Data position               
            len = 2;
            subStr = payload.Substring(index, len);

            string payloadStr;
            if (version < 3)
            {
                switch (subStr)
                {
                    case "00":
                        payloadStr = "Start";
                        break;
                    case "01":
                        payloadStr = "Intermediate";
                        break;
                    case "02":
                        payloadStr = "Finish";
                        break;
                    default:
                        payloadStr = "Unknown data log message";
                        break;
                }

            }
            else
            {
                switch (subStr)
                {
                    case "00":
                        payloadStr = "Start";
                        break;
                    case "FF":
                    case "ff":
                        payloadStr = "Finish";
                        break;
                    default:
                        payloadStr = "Intermediate " + subStr;
                        break;
                }

            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;

            // no contents
            // no RSSI
            

            return sb.ToString();
        }


        public string parseAccelWaveform(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            //String dataLogPayload;
            String subStr;
            int len = 2;
            string payloadStr;

            sb.Append("Acceleration Waveform");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();

            /*
            // Axis_Data_Select
            len = 1;
            subStr = payload.Substring(index, len); // 


            switch (subStr)
            {
                case "0":
                    payloadStr = "XYZ";
                    break;
                case "1":
                    payloadStr = "X";
                    break;
                case "2":
                    payloadStr = "Y";
                    break;
                case "3":
                    payloadStr = "Z";
                    break;
                default:
                    payloadStr = "Unknown Axis_Data_Select message";
                    break;
            }

            sb.AppendFormat("Selected data axis: {0}", payloadStr);
            

            sb.AppendLine();
            index += len;

            // Identification
            len = 1;
            subStr = payload.Substring(index, len); // 
            
            sb.AppendFormat("Identification: 0x{0}", subStr);
            sb.AppendLine();
            index += len;
            */



            // Acceleration Waveform Control Header 
            len = 2;
            subStr = payload.Substring(index, len);

            //SByte ctrlByte = Convert.ToSByte(subStr);            
            Byte ctrlByte = byte.Parse(subStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);


            string bitFlag = Convert.ToString(ctrlByte, 2).PadLeft(8, '0'); // 

            var chars = bitFlag.ToCharArray();

            
            // BMA Acceleration g range 
            sb.Append("Acceleration g range: ");

            if ((chars[0].Equals('0')) && (chars[1].Equals('0')))
                sb.Append("2g");
            else if ((chars[0].Equals('0')) && (chars[1].Equals('1')))
                sb.Append("4g");
            else if ((chars[0].Equals('1')) && (chars[1].Equals('0')))
                sb.Append("8g");
            else //((chars[0].Equals('1')) && (chars[1].Equals('1')))
                sb.Append("16g");

            // Next line 
            sb.AppendLine();


            // Selected Data Axis 
            sb.Append("Selected axis: ");

            if ((chars[2].Equals('0')) && (chars[3].Equals('0')))
                sb.Append("XYZ");
            else if ((chars[2].Equals('0')) && (chars[3].Equals('1')))
                sb.Append("X");
            else if ((chars[2].Equals('1')) && (chars[3].Equals('0')))
                sb.Append("Y");
            else //((chars[0].Equals('1')) && (chars[1].Equals('1')))
                sb.Append("Z");

            // Next line 
            sb.AppendLine();


            // Identification 
            byte y = 0x0F;
            int identificationNumber = 1;

            identificationNumber = (int)(ctrlByte & y);

            sb.AppendFormat("Identification : {0}", identificationNumber);
            sb.AppendLine();


            index += len;



            // Data position      
            /*
            len = 2;
            subStr = payload.Substring(index, len);

            switch (subStr)
            {
                case "00":
                    payloadStr = "Start";
                    break;
                case "FF":
                case "ff":
                    payloadStr = "Finish";
                    break;
                default:
                    payloadStr = "Intermediate 0x" + subStr;
                    break;
            }

    
            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;
            */



            // Packing type : 12bit or 16bit              
            len = 1;
            subStr = payload.Substring(index, len);

            sb.Append("Packing type: ");

            switch (subStr)
            {
                case "1":
                    payloadStr = "12bit packed";
                    break;
                case "0":
                case "F":
                case "f":
                    payloadStr = "16bit aligned";
                    break;
                default:
                    payloadStr = "Unknown alignment 0x" + subStr;
                    break;
            }


            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Data position               
            len = 1;
            subStr = payload.Substring(index, len);

            switch (subStr)
            {
                case "0":
                    payloadStr = "Start";
                    break;
                case "F":
                case "f":
                    payloadStr = "Finish";
                    break;
                default:
                    payloadStr = "Intermediate 0x" + subStr;
                    break;
            }


            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;



            // Show Accel Waveform data    
            
            if (index + 96 > payload.Length)
                len = 72;  // AS923. 36Byte 
            else
                len = 96; // SKTelecom 48Byte 

            sb.Append("Acc data: ");

            for (int i = 0; i < len; i += 2)
            {
                subStr = payload.Substring(index, 2);
                index += 2;
                sb.AppendFormat("0x{0}, ", subStr);
            }

            sb.AppendLine();
            index += len;

            // no RSSI


            return sb.ToString();
        }


        public string parseInclination(string payload, int version)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String subStr;
            int len = 2;

            sb.Append("Inclination");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();


            // Inclination difference of X axis 
            len = 4;
            subStr = payload.Substring(index, len);
            int xAngleDiff = Convert.ToInt16(subStr, 16);
            double xdegree = ((double)xAngleDiff) / 100;
            sb.AppendFormat("Diff. of X: {0} deg", xdegree.ToString("N2"));
            sb.AppendLine();
            index += len;


            // Inclination difference of Y axis 
            len = 4;
            subStr = payload.Substring(index, len);
            int yAngleDiff = Convert.ToInt16(subStr, 16);
            double ydegree = ((double)yAngleDiff) / 100;
            sb.AppendFormat("Diff. of Y: {0} deg", ydegree.ToString("N2"));
            sb.AppendLine();
            index += len;

            // Inclination difference of Z axis 
            len = 4;
            subStr = payload.Substring(index, len);
            int zAngleDiff = Convert.ToInt16(subStr, 16);
            double zdegree = ((double)zAngleDiff) / 100;
            sb.AppendFormat("Diff. of Z: {0} deg", zdegree.ToString("N2"));
            sb.AppendLine();
            index += len;

            
            return sb.ToString();
        }

        public string parseMachineRuntimeMeasurement(string payload, int version, UpstreamCommon.DeviceType devType)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String subStr;
            int len = 2;

            sb.Append("Machine Runtime Measurement");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();


            // Protocol Version 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            int protocolVersion = Convert.ToUInt16(subStr, 16);      
            sb.AppendFormat("Protocol Version: {0}", protocolVersion.ToString());
            sb.AppendLine();
            index += len;


            // Machine state  
            len = 1 * 2;
            subStr = payload.Substring(index, len);

            string payloadStr;
            switch (subStr)
            {
                case "00":
                    payloadStr = "Commissioning State";
                    break;
                case "01":
                    payloadStr = "Inactive State";
                    break;
                case "02":
                    payloadStr = "Active State";
                    break;
                default:
                    payloadStr = "Unknown state";
                    break;
            }

            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Parse machine report period 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtReportPeriod = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Report Period: {0} min", mrtReportPeriod);
            sb.AppendLine();

            index += len;


            // Parse REPORT packet as device type 
            len = 9 * 2;
            subStr = payload.Substring(index, len);            
            payloadStr = parseReport(subStr, version, devType);
            sb.Append(payloadStr);
            sb.AppendLine();
            index += len;


            // Parse machine runtime duration 
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtRuntimeDuration = Convert.ToUInt16(subStr, 16);          
            sb.AppendFormat("Runtime Duration: {0} min", mrtRuntimeDuration);
            sb.AppendLine();

            index += len;

            // Parse machine operation threshold 
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtOperationThreshold = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Operation Threshold: {0} mg", mrtOperationThreshold);
            sb.AppendLine();

            index += len;


            // Parse machine shock threshold 
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtShockThreshold = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Shock Threshold: {0} mg", mrtShockThreshold);
            sb.AppendLine();

            index += len;


            // Parse machine timestamp 
            len = 4 * 2;
            subStr = payload.Substring(index, len);
            UInt32 mrtTimestamp = Convert.ToUInt32(subStr, 16);
            sb.AppendFormat("Timestamp: {0}", mrtTimestamp);
            sb.AppendLine();

            index += len;


            // Parse battery level 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            UInt16 batt_level = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Battery level: {0} %", batt_level);
            sb.AppendLine();

            index += len;


            // Parse accelerometer status 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            UInt16 acc_status = Convert.ToUInt16(subStr, 16);
            if (acc_status == 1)
                sb.AppendFormat("Accelerometer status OK");
            else
                sb.AppendFormat("Accelerometer status FAIL");

            sb.AppendLine();

            index += len;


            // reserved  
            len = 4 * 2;
            subStr = payload.Substring(index, len);
            sb.AppendFormat("Reserved {0}", subStr);
            sb.AppendLine();

            index += len;
            
            return sb.ToString();
        }

        public string parseMachineRuntimeReport(string payload, int version, UpstreamCommon.DeviceType devType)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            String subStr;
            int len = 2;

            sb.Append("Machine Runtime Report");
            sb.AppendLine();

            // Trim whitespace
            payload = payload.Trim();


            // Protocol Version 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            int protocolVersion = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Protocol Version: {0}", protocolVersion.ToString());
            sb.AppendLine();
            index += len;


            // Parse battery level 
            len = 1 * 2;
            subStr = payload.Substring(index, len);
            UInt16 batt_level = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Battery level: {0} %", batt_level);
            sb.AppendLine();

            index += len;


            // Parse accumulated machine runtime duration 
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtAccRuntimeDuration = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Accumulated Runtime: {0} min.", mrtAccRuntimeDuration);
            sb.AppendLine();

            index += len;

            // Parse runtime observation duration  
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            UInt16 mrtRuntimeObservationTime = Convert.ToUInt16(subStr, 16);
            sb.AppendFormat("Runtime Observation Time: {0} min.", mrtRuntimeObservationTime);
            sb.AppendLine();

            index += len;
            

            // reserved  
            len = 2 * 2;
            subStr = payload.Substring(index, len);
            sb.AppendFormat("Reserved {0}", subStr);
            sb.AppendLine();

            index += len;

            return sb.ToString();
        }

    }
}
