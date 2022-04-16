using System;
using System.Collections.Generic;


namespace BEISCH
{
    class Program
    {
        static void Main(string[] args)
        {
            int numkeys;
            Random rnd = new Random();
            bool loop = true;
            do //Get the number of keys to be inserted from the user and check if it is a valid number
            {
                Console.WriteLine("Please enter the number of keys you want to insert (Please enter 900 at most)");
                numkeys = Convert.ToInt32(Console.ReadLine());
                if (numkeys < 0)
                    Console.WriteLine("Please enter a positive number");
                else if (numkeys > 900)
                    Console.WriteLine("The number of keys to be inserted should be equal to or less than 900");
                else //The number of keys entered is suitable. So break the loop
                    loop = false;
                //break;
            } while (loop);
            Table table = new Table(997);
            TableBT table_bt = new TableBT(997);
            FillBlanks(table_bt);
            int num;
            int top_empty_index = 0; //The first index that is empty in the array
            int bottom_empty_index = 996; //The last index that is empty in the array
            bool top_or_bottom = true; //true=insert to top;  false=insert to bottom
            for (int i = 1; i <= numkeys; i++)
            {
                num = rnd.Next();
                //num = Convert.ToInt32(Console.ReadLine());
                BEISCH(i, num, ref top_empty_index, ref bottom_empty_index, ref top_or_bottom, table);
                BinaryTree(i, num, table_bt);
            }
            FillBlanks(table);
            ProbesForBEISCH(table);
            Console.WriteLine("____________________________________________________________________________________________\nBEISCH Table");
            PrintTable(table);
            Console.WriteLine("____________________________________________________________________________________________\n" +
                "Binary Tree Table");
            PrintTable(table_bt);
            table.AverageNumberOfProbes();
            table_bt.AverageNumberOfProbes();
            Console.Write("______________________________________________________________________" +
                "\nPlease enter the value you want to search in the tables:");
            int value = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            SearchValue(value, table, table_bt);
        }
        public static void ProbesForBEISCH(Table table)
        {
            for(int i=0;i<997;i++)
            {
                int k = 1;
                int search = table.records[i].getValue();
                int index=Hash(table.records[i].getValue());
                if (index != -1)
                {
                    while (table.records[index].getValue() != search)
                    {
                        k++;
                        index = table.records[index].getLink();
                        if (index == -1)
                        {
                            break;
                        }
                    }
                }
                table.records[i].setProbe(k);
            }
        }
        public static void SearchValue(int value, Table table, TableBT table_bt)
        {
            SearchInBEISCH(value, table);
            SearchInBinaryTree(value, table_bt);
        }
        public static void SearchInBEISCH(int value, Table table)
        {
            int index = Hash(value);
            int probe = 1;
            while(table.records[index].getValue()!=value)
            {
                index = table.records[index].getLink();
                if (index == -1)
                {
                    Console.WriteLine("The value you search for does not exist in BEISCH table");
                    return;
                }
                probe++;
            }
            Console.WriteLine($"The value you are searching for is on {index}. index in BEISCH table and it is found in {probe} probe(s)");
        }
        public static void SearchInBinaryTree(int value, TableBT table_bt)
        {
            int index = Hash(value);
            int step_size = Quotient(value);
            int i = 1;
            while(table_bt.records[index].getValue()!=value && i<=997)
            {
                index = Hash(index + step_size);
                i++;
            }
            if(i==998)
            {
                Console.WriteLine("The value you search for does not exist in Binary Tree table");
                return;
            }
            Console.WriteLine($"The value you are searching for is on {index}. index in Binary Tree table and it is found in {i} probe(s)");
        }
        public static void BEISCH(int i, int num, ref int top_empty_index, ref int bottom_empty_index, ref bool top_or_bottom, Table table)
        {
            if (top_empty_index <= bottom_empty_index)
            {
                //num = Convert.ToInt32(Console.ReadLine());
                int home = Hash(num);
                if (table.records[home] == null) //Home address is empty
                {
                    table.records[home] = new Record(i, num);
                    UpdateTopEmtyIndex(ref top_empty_index, bottom_empty_index, table.records);
                    UpdateBottomEmptyIndex(ref bottom_empty_index, top_empty_index, table.records);
                }
                else //Home address is not empty
                {
                    if (top_or_bottom) //Add record on top
                    {
                        table.records[top_empty_index] = new Record(i, num, table.records[home].getLink());
                        table.records[home].setLink(top_empty_index);
                        UpdateProbes(home, table);
                        UpdateTopEmtyIndex(ref top_empty_index, bottom_empty_index, table.records);
                        top_or_bottom = false;
                    }
                    else //Add record on bottom
                    {
                        table.records[bottom_empty_index] = new Record(i, num, table.records[home].getLink());
                        table.records[home].setLink(bottom_empty_index);
                        UpdateProbes(home, table);
                        UpdateBottomEmptyIndex(ref bottom_empty_index, top_empty_index, table.records);
                        top_or_bottom = true;
                    }
                }
            }
            else
            {
                Console.Write("There is either only one empty space or no place to insert a new record!!!");
            }
        }
        public static void BinaryTree(int i, int num, TableBT table_bt)
        {
            int table_index = Hash(num);
            if (table_bt.records[table_index].getValue() == -1)
            {
                RecordBT new_record = new RecordBT(i, num);
                table_bt.records[table_index] = new_record;
            }
            else
            {
                bool l_or_r_child = false; //If true: left child; else: right child
                int step_size = Quotient(num);
                List<TreeElement> BinaryTree = new List<TreeElement>();
                BinaryTree.Add(new TreeElement(table_index, l_or_r_child, step_size));
                int j = 0;
                int parent = -1;
                bool is_l_or_r = false;
                while (table_bt.records[BinaryTree[j].table_index].getValue() != -1)
                {
                    if (BinaryTree[j].l_or_r_child == false)
                    {
                        parent += 1;
                    }
                    if (is_l_or_r == false) //If the node is a left child
                    {
                        is_l_or_r = true;
                        BinaryTree.Add(new TreeElement(Hash(BinaryTree[parent].table_index + BinaryTree[parent].step_size), is_l_or_r, BinaryTree[parent].step_size));
                    }
                    else //If the node is a right child
                    {
                        is_l_or_r = false;
                        BinaryTree.Add(new TreeElement(Hash(BinaryTree[parent].table_index + Quotient(table_bt.records[BinaryTree[parent].table_index].getValue())), is_l_or_r, Quotient(table_bt.records[BinaryTree[parent].table_index].getValue())));

                    }
                    j += 1;
                    if (table_bt.records[BinaryTree[j].table_index].getValue() == -1)
                    {
                        ReturnedElement value_to_fill = FillEmpty(table_bt, BinaryTree, j, num);
                        RecordBT new_record = new RecordBT(i, value_to_fill.carried);
                        new_record.setProbe(value_to_fill.probe);
                        table_bt.records[BinaryTree[j].table_index] = new_record;
                        break;
                    }
                }
            }
        }
        public static ReturnedElement FillEmpty(TableBT table_bt, List<TreeElement> BinaryTree, int j, int num)
        {
            ReturnedElement returned;
            if (j == 0)
            {
                returned = new ReturnedElement();
                returned.carried = num;
                returned.probe = 1;
                return returned;
            }
            else
            {
                if (BinaryTree[j].l_or_r_child == true) //If left child
                {
                    returned = FillEmpty(table_bt, BinaryTree, (j - 1) / 2, num);
                    returned.probe += 1;
                    return returned;
                }
                else //If right child
                {
                    returned = FillEmpty(table_bt, BinaryTree, (j - 2) / 2, num);
                    int fill_parent = returned.carried;
                    int fill_parent_probe = returned.probe;
                    returned.carried = table_bt.records[BinaryTree[(j - 2) / 2].table_index].getValue();
                    returned.probe = table_bt.records[BinaryTree[(j - 2) / 2].table_index].getProbe();
                    table_bt.records[BinaryTree[(j - 2) / 2].table_index].setValue(fill_parent);
                    table_bt.records[BinaryTree[(j - 2) / 2].table_index].setProbe(fill_parent_probe);
                    returned.probe += 1;
                    return returned;
                }
            }
        }
        public static void FillBlanks(Table table)
        {
            for (int i = 0; i < 997; i++)
            {
                if (table.records[i] == null)
                {
                    table.records[i] = new Record();
                }
            }
        }
        public static void FillBlanks(TableBT table)
        {
            for (int i = 0; i < 997; i++)
            {
                if (table.records[i] == null)
                {
                    table.records[i] = new RecordBT();
                }
            }
        }
        public static void PrintTable(Table table)
        {
            Console.WriteLine("____________________________________________________________________________________________" +
                "\nIndex\tInsertion Order\t\tValue\t\t\tLink\t\tProbes");
            for (int i = 0; i < 997; i++)
            {
                if (table.records[i].getInsertionOrder() != -1)
                {
                    Console.WriteLine("{0}\t{1}\t\t\t{2,-19}\t{3}\t\t{4}", i, table.records[i].getInsertionOrder(), table.records[i].getValue(), table.records[i].getLink(), table.records[i].getProbe());
                }
            }
        }
        public static void PrintTable(TableBT table)
        {
            Console.WriteLine("____________________________________________________________________________________________" +
                "\nIndex\tInsertion Order\t\tValue\t\t\tProbes");
            for (int i = 0; i < 997; i++)
            {
                if (table.records[i].getInsertionOrder() != -1)
                {
                    Console.WriteLine("{0}\t{1}\t\t\t{2,-19}\t{3}", i, table.records[i].getInsertionOrder(), table.records[i].getValue(), table.records[i].getProbe());
                }            
            }
        }
        public static void UpdateProbes(int home, Table table)
        {
            Record chain_record = table.records[table.records[home].getLink()];
            if (Hash(table.records[home].getValue()) == home)
            {
                while (chain_record.getLink() != -1)
                {
                    chain_record = table.records[chain_record.getLink()];
                    if (Hash(chain_record.getValue()) == home)
                    {
                        chain_record.setProbe(chain_record.getProbe() + 1);
                    }
                }
            }
            else
            {
                while (chain_record.getLink() != -1)
                {
                    chain_record = table.records[chain_record.getLink()];
                    chain_record.setProbe(chain_record.getProbe() + 1);
                }
            }
        }
        public static void UpdateTopEmtyIndex(ref int top_empty_index, int bottom_empty_index, Record[] records)
        {
            while (records[top_empty_index] != null && top_empty_index < bottom_empty_index)
            {
                top_empty_index++;
            }
        }
        public static void UpdateBottomEmptyIndex(ref int bottom_empty_index, int top_empty_index, Record[] records)
        {
            while (records[bottom_empty_index] != null && top_empty_index < bottom_empty_index)
            {
                bottom_empty_index--;
            }
        }
        public static int Hash(int x)
        {
            int hashed = x % 997;
            return hashed;
        }
        public static int Quotient(int x)
        {
            int quotient = x / 997;
            return quotient;
        }
    }
    class Table
    {
        public Record[] records;
        private int table_size;
        public Table(int table_size)
        {
            records = new Record[table_size];
            this.table_size = table_size;
        }
        public void AverageNumberOfProbes()
        {
            int total_of_probes = 0;
            double filled=0;
            double average_of_probes;
            for(int i=0;i<table_size;i++)
            {
                if(records[i].getInsertionOrder()!=-1)
                {
                    total_of_probes += records[i].getProbe();
                    filled++;
                }
            }
            average_of_probes = total_of_probes / filled;
            Console.WriteLine("\nThe average of probes is {0}", average_of_probes);
        }
    }
    class Record
    {
        private int value;
        private int link;
        private int insertion_order;
        private int probe;
        public Record()
        {
            value = -1;
            link = -1;
            insertion_order = -1;
            probe = 0;
        }
        public Record(int insertion_order, int value)
        {
            this.insertion_order = insertion_order;
            this.value = value;
            link = -1;
            probe = 1;
        }
        public Record(int insertion_order, int value, int link)
        {
            this.insertion_order = insertion_order;
            this.value = value;
            this.link = link;
            probe = 2;
        }
        public void setInsertionOrder(int new_insertion_order)
        {
            insertion_order = new_insertion_order;
        }
        public void setProbe(int new_probe)
        {
            probe = new_probe;
        }
        public void setValue(int new_value)
        {
            value = new_value;
        }
        public void setLink(int new_link)
        {
            link = new_link;
        }
        public int getInsertionOrder()
        {
            return insertion_order;
        }
        public int getProbe()
        {
            return probe;
        }
        public int getValue()
        {
            return value;
        }
        public int getLink()
        {
            return link;
        }
    }
    class ReturnedElement
    {
        public int carried;
        public int probe;
    }
    class TableBT
    {
        public RecordBT[] records;
        private int table_size;
        public TableBT(int table_size)
        {
            records = new RecordBT[table_size];
            this.table_size = table_size;
        }
        public void AverageNumberOfProbes()
        {
            int total_of_probes = 0;
            double filled = 0;
            double average_of_probes;
            for (int i = 0; i < table_size; i++)
            {
                if (records[i].getInsertionOrder() != -1)
                {
                    total_of_probes += records[i].getProbe();
                    filled++;
                }
            }
            average_of_probes = total_of_probes / filled;
            Console.WriteLine("\nThe average of probes is {0}", average_of_probes);
        }
    }
    class TreeElement
    {
        public int table_index;
        public bool l_or_r_child;
        public int step_size;
        public TreeElement(int table_index, bool l_or_r_child, int step_size)
        {
            this.table_index = table_index;
            this.l_or_r_child = l_or_r_child;
            this.step_size = step_size;
        }
    }
    class RecordBT
    {
        private int value;
        private int insertion_order;
        private int probe;
        public RecordBT()
        {
            value = -1;
            insertion_order = -1;
            probe = 0;
        }
        public RecordBT(int insertion_order, int value)
        {
            this.insertion_order = insertion_order;
            this.value = value;
            probe = 1;
        }
        public void setInsertionOrder(int new_insertion_order)
        {
            insertion_order = new_insertion_order;
        }
        public void setProbe(int new_probe)
        {
            probe = new_probe;
        }
        public void setValue(int new_value)
        {
            value = new_value;
        }
        public int getInsertionOrder()
        {
            return insertion_order;
        }
        public int getProbe()
        {
            return probe;
        }
        public int getValue()
        {
            return value;
        }
    }
}
