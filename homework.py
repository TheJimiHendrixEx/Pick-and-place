'''
number = 20
second_number = 10
first_array = [1,5]
second_array = [1,2,3]
if number>15:
    print("1")
if len(first_array) == 2:
    print("2")
if len(second_array) == 3:
    print("3")
if len(first_array) + len(second_array) == 5:
    print("4")
if first_array and first_array[0] == 1:
    print("5")
if not second_number == 0:
    print("6")
'''

'''
numbers = [
    951, 402, 984, 651, 360, 69, 408, 319, 601, 485, 980, 507, 725, 547, 544,
    615, 83, 165, 141, 501, 263, 617, 865, 575, 219, 390, 984, 592, 236, 105, 942, 941,
    386, 462, 47, 418, 907, 344, 236, 375, 823, 566, 597, 978, 328, 615, 953, 345,
    399, 162, 758, 219, 918, 237, 412, 566, 826, 248, 866, 950, 626, 949, 687, 217,
    815, 67, 104, 58, 512, 24, 892, 894, 767, 553, 81, 379, 843, 831, 445, 742, 717,
    958, 609, 842, 451, 688, 753, 854, 685, 93, 857, 440, 380, 126, 721, 328, 753, 470,
    743, 527
]

# your code goes here

#for i in range(numbers[0,53]):
for i in numbers[0:54]:
    if (i%2==0):
        print(i) 
'''

'''
a = [1,2]
a.append(10)
a.append(69)
print(a)
'''

'''
data = []
n = int(input('Enter how many elements you want: '))
for i in range(0, n):
    x = int(input('Enter the numbers into the array: '))
    data.append(x*.65)

print(data)
'''

'''
A = int(input('Enter: '))
B = int(input('Enter: '))
if A<B:
    print()
'''

'''
n = input('Enter: ')
numcount = 0
for num in n.split():
    if (num == "0"):
        numcount += 1
print(numcount)
'''
 

#Set
cs_courses = {'History','MAth','Physic','CompSci'} #set doesn't care about the order 
cs_courses.join('XD')
print(cs_courses)
